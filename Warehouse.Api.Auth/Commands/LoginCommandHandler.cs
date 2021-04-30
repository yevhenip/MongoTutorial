using System;
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
    public record LoginCommand(LoginDto Login) : IRequest<Result<UserAuthenticatedDto>>;

    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<UserAuthenticatedDto>>
    {
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly IPasswordHasher<UserDto> _hasher;
        private readonly JwtTokenConfiguration _tokenConfiguration;

        public LoginCommandHandler(IMapper mapper, ISender sender, IUserRepository userRepository,
            IOptions<JwtTokenConfiguration> tokenConfiguration, IRefreshTokenRepository tokenRepository,
            IPasswordHasher<UserDto> hasher)
        {
            _mapper = mapper;
            _sender = sender;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _hasher = hasher;
            _tokenConfiguration = tokenConfiguration.Value;
        }

        public async Task<Result<UserAuthenticatedDto>> Handle(LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.UserName == request.Login.UserName);

            if (user == null)
            {
                throw Result<User>.Failure("userName", "Invalid userName", HttpStatusCode.BadRequest);
            }

            user.SessionId = Guid.NewGuid().ToString();
            var userDto = _mapper.Map<UserDto>(user);

            IsValid(userDto, request.Login.Password);

            var jwtToken = CommandExtensions.GenerateJwtToken(userDto, _tokenConfiguration);
            var tokenString = CommandExtensions.GenerateRefreshToken();
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.RefreshTokenExpirationMinutes),
                Token = tokenString,
                User = user
            };

            await _tokenRepository.CreateAsync(refreshToken);
            await _sender.PublishAsync(new UpdatedUser(user), cancellationToken);
            await _sender.PublishAsync(new CreatedToken(refreshToken), cancellationToken);
            UserAuthenticatedDto authenticatedDto = new(userDto, jwtToken, refreshToken.Token);
            return Result<UserAuthenticatedDto>.Success(authenticatedDto);
        }

        private void IsValid(UserDto user, string password)
        {
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (result is PasswordVerificationResult.Failed)
            {
                throw Result<UserAuthenticatedDto>.Failure("password", "Invalid password", HttpStatusCode.BadRequest);
            }
        }
    }
}