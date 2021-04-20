using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Warehouse.Api.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings;
using Warehouse.Domain;

namespace Warehouse.Api.Auth.Business
{
    public class AuthService : ServiceBase<RefreshToken>, IAuthService
    {
        private readonly JwtTokenConfiguration _tokenConfiguration;
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<UserDto> _hasher;

        public AuthService(IOptions<JwtTokenConfiguration> tokenConfiguration, IRefreshTokenRepository tokenRepository,
            IDistributedCache distributedCache, IMapper mapper, IUserRepository userRepository, IBus bus,
            IPasswordHasher<UserDto> hasher) : base(mapper, distributedCache, bus)
        {
            _tokenConfiguration = tokenConfiguration.Value;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _hasher = hasher;
        }

        public async Task<Result<UserAuthenticatedDto>> RegisterAsync(RegisterDto register)
        {
            var user = Mapper.Map<UserDto>(register);
            await IsValid(user);
            var hashedPassword = _hasher.HashPassword(user, register.Password);
            user = user with {PasswordHash = hashedPassword, Roles = new List<string> {"User"}};
            var userToDb = Mapper.Map<User>(user);
            var jwtToken = GenerateJwtToken(user);
            var tokenString = GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.RefreshTokenExpirationMinutes),
                Token = tokenString,
                User = userToDb
            };

            await _tokenRepository.CreateAsync(refreshToken);
            await Bus.PubSub.PublishAsync(userToDb);
            UserAuthenticatedDto authenticatedDto = new(user, jwtToken, refreshToken.Token);
            return Result<UserAuthenticatedDto>.Success(authenticatedDto);
        }

        public async Task<Result<UserAuthenticatedDto>> LoginAsync(LoginDto login, string sessionId)
        {
            var user = await _userRepository.GetByUserNameAsync(login.UserName);
            if (user == null)
            {
                throw Result<User>.Failure("userName", "Invalid userName", HttpStatusCode.BadRequest);
            }

            user.SessionId = sessionId;
            var userDto = Mapper.Map<UserDto>(user);

            IsValid(userDto, login.Password);

            await Bus.PubSub.PublishAsync(user);

            var jwtToken = GenerateJwtToken(userDto);
            var tokenString = GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.RefreshTokenExpirationMinutes),
                Token = tokenString,
                User = user
            };

            await _tokenRepository.CreateAsync(refreshToken);
            UserAuthenticatedDto authenticatedDto = new(userDto, jwtToken, refreshToken.Token);
            return Result<UserAuthenticatedDto>.Success(authenticatedDto);
        }

        public async Task<Result<UserAuthenticatedDto>> RefreshTokenAsync(string userId, TokenDto token,
            string sessionId)
        {
            var user = await _userRepository.GetAsync(userId);
            user.SessionId = sessionId;
            var userDto = Mapper.Map<UserDto>(user);

            var refreshTokenInDb = await _tokenRepository.GetAsync(userId, token.Name);

            CheckForNull(refreshTokenInDb);
            IsValid(refreshTokenInDb);

            await Bus.PubSub.PublishAsync(user);

            var jwtToken = GenerateJwtToken(userDto);
            var tokenString = GenerateRefreshToken();
            var userInDb = Mapper.Map<User>(user);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.RefreshTokenExpirationMinutes),
                Token = tokenString,
                User = userInDb
            };

            await _tokenRepository.CreateAsync(refreshToken);
            await _tokenRepository.DeleteAsync(refreshTokenInDb.Id);
            UserAuthenticatedDto authenticatedDto = new(userDto, jwtToken, refreshToken.Token);
            return Result<UserAuthenticatedDto>.Success(authenticatedDto);
        }

        public async Task<Result<object>> LogoutAsync(string userId)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw Result<User>.Failure("id", "Invalid id");
            }

            user.SessionId = null;
            await Bus.PubSub.PublishAsync(user);
            return Result<object>.Success();
        }

        private void IsValid(UserDto user, string password)
        {
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result is PasswordVerificationResult.Failed)
            {
                throw Result<UserAuthenticatedDto>.Failure("password", "Invalid password", HttpStatusCode.BadRequest);
            }
        }

        private async Task IsValid(UserDto user)
        {
            var userFromDb = await _userRepository.GetByUserNameAsync(user.UserName);
            if (userFromDb is not null)
            {
                throw Result<UserAuthenticatedDto>.Failure("userName", "User with such userName already exists",
                    HttpStatusCode.BadRequest);
            }

            userFromDb = await _userRepository.GetByEmailAsync(user.Email);

            if (userFromDb is not null)
            {
                throw Result<UserAuthenticatedDto>.Failure("email", "User with such email already exists",
                    HttpStatusCode.BadRequest);
            }
        }

        private static void IsValid(RefreshToken token)
        {
            if (token.DateExpires <= DateTime.UtcNow)
            {
                throw Result<RefreshToken>.Failure("token", "Token is expired", HttpStatusCode.BadRequest);
            }
        }

        private string GenerateJwtToken(UserDto user)
        {
            List<Claim> claims = new()
            {
                new("Id", user.Id),
                new("UserName", user.UserName),
                new("Email", user.Email),
                new("SessionId", user.SessionId)
            };
            claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var jwtToken = new JwtSecurityToken(
                _tokenConfiguration.Issuer,
                _tokenConfiguration.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_tokenConfiguration.AccessTokenExpirationMinutes),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenConfiguration.Secret)),
                    SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}