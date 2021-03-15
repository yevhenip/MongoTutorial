using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Warehouse.Api.Extensions;
using Warehouse.Core.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Messaging.Sender;
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
        private readonly ISender _sender;

        public AuthService(IOptions<JwtTokenConfiguration> tokenConfiguration, IRefreshTokenRepository tokenRepository,
            IDistributedCache distributedCache, IMapper mapper, IUserRepository userRepository, ISender sender,
            IPasswordHasher<UserDto> hasher) : base(distributedCache, mapper, null)
        {
            _tokenConfiguration = tokenConfiguration.Value;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _sender = sender;
            _hasher = hasher;
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto register)
        {
            var user = Mapper.Map<UserDto>(register);
            var hashedPassword = _hasher.HashPassword(user, register.Password);
            user = user with {PasswordHash = hashedPassword, Roles = new List<string> {"User"}};
            await _sender.SendMessage(user, Queues.CreateUserQueue);
            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserAuthenticatedDto>> LoginAsync(LoginDto login, string sessionId)
        {
            var user = await _userRepository.GetByUserNameAsync(login.UserName);
            if (user == null)
            {
                throw Result<User>.Failure("userName", "Invalid userName");
            }

            user.SessionId = sessionId;
            var userDto = Mapper.Map<UserDto>(user);

            IsValid(userDto, login.Password);

            await _sender.SendMessage(user, Queues.UpdateUserQueue);

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

            await _sender.SendMessage(user, Queues.UpdateUserQueue);

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
            await _sender.SendMessage(user, Queues.UpdateUserQueue);
            return Result<object>.Success();
        }

        private void IsValid(UserDto user, string password)
        {
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result is PasswordVerificationResult.Failed)
            {
                throw Result<UserAuthenticatedDto>.Failure("password", "Invalid password");
            }
        }

        private static void IsValid(RefreshToken token)
        {
            if (token.DateExpires <= DateTime.UtcNow)
            {
                throw Result<RefreshToken>.Failure("token", "Token is expired");
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