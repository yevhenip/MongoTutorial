using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Settings;
using Warehouse.Domain;
using ISender = Warehouse.Core.Interfaces.Services.ISender;

namespace Warehouse.Api.Auth.Commands
{
    public record RegisterCommand(RegisterDto Register) : IRequest<Result<UserAuthenticatedDto>>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<UserAuthenticatedDto>>
    {
        private readonly IPasswordHasher<UserDto> _hasher;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenConfiguration _tokenConfiguration;

        public RegisterCommandHandler(IPasswordHasher<UserDto> hasher, IMapper mapper, ISender sender,
            IRefreshTokenRepository tokenRepository, IUserRepository userRepository,
            IOptions<JwtTokenConfiguration> tokenConfiguration)
        {
            _hasher = hasher;
            _mapper = mapper;
            _sender = sender;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _tokenConfiguration = tokenConfiguration.Value;
        }

        public async Task<Result<UserAuthenticatedDto>> Handle(RegisterCommand request,
            CancellationToken cancellationToken)
        {
            var user = _mapper.Map<UserDto>(request.Register);
            await IsValid(user);
            var hashedPassword = _hasher.HashPassword(user, request.Register.Password);
            user = user with {PasswordHash = hashedPassword, Roles = new List<string> {"User"}};
            var userToDb = _mapper.Map<User>(user);
            var jwtToken = JwtExtensions.GenerateJwtToken(user, _tokenConfiguration);
            var tokenString = JwtExtensions.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.RefreshTokenExpirationMinutes),
                Token = tokenString,
                User = userToDb
            };

            await _tokenRepository.CreateAsync(refreshToken);
            UserAuthenticatedDto authenticatedDto = new(user, jwtToken, refreshToken.Token);
            await _sender.PublishAsync(new CreatedUser(userToDb), cancellationToken);
            await _sender.PublishAsync(new CreatedToken(refreshToken), cancellationToken);

            return Result<UserAuthenticatedDto>.Success(authenticatedDto);
        }

        private async Task IsValid(UserDto user)
        {
            var userFromDb = await _userRepository.GetAsync(u => u.UserName == user.UserName);
            if (userFromDb is not null)
            {
                throw Result<UserAuthenticatedDto>.Failure("userName", "User with such userName already exists",
                    HttpStatusCode.BadRequest);
            }

            userFromDb = await _userRepository.GetAsync(u => u.Email == user.Email);

            if (userFromDb is not null)
            {
                throw Result<UserAuthenticatedDto>.Failure("email", "User with such email already exists",
                    HttpStatusCode.BadRequest);
            }
        }
    }
}