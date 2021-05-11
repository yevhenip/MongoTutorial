using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
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
    public record RefreshTokenCommand(string UserId, TokenDto Token) : IRequest<Result<UserAuthenticatedDto>>;

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<UserAuthenticatedDto>>
    {
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenConfiguration _tokenConfiguration;

        public RefreshTokenCommandHandler(IRefreshTokenRepository tokenRepository, IMapper mapper, ISender sender,
            IUserRepository userRepository, IOptions<JwtTokenConfiguration> tokenConfiguration)
        {
            _tokenRepository = tokenRepository;
            _mapper = mapper;
            _sender = sender;
            _userRepository = userRepository;
            _tokenConfiguration = tokenConfiguration.Value;
        }

        public async Task<Result<UserAuthenticatedDto>> Handle(RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Id == request.UserId);
            user.SessionId = Guid.NewGuid().ToString();
            var userDto = _mapper.Map<UserDto>(user);

            var refreshTokenInDb = await
                _tokenRepository.GetAsync(t => t.User.Id == request.UserId && t.Token == request.Token.Name);

            refreshTokenInDb.CheckForNull();
            IsValid(refreshTokenInDb);

            var jwtToken = JwtExtensions.GenerateJwtToken(userDto, _tokenConfiguration);
            var tokenString = JwtExtensions.GenerateRefreshToken();
            var userInDb = _mapper.Map<User>(user);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow,
                DateExpires = DateTime.UtcNow.AddMinutes(_tokenConfiguration.RefreshTokenExpirationMinutes),
                Token = tokenString,
                User = userInDb
            };

            await _tokenRepository.CreateAsync(refreshToken);
            await _tokenRepository.DeleteAsync(t => t.Id == refreshTokenInDb.Id);
            await _sender.PublishAsync(new UpdatedUser(user), cancellationToken);
            await _sender.PublishAsync(new CreatedToken(refreshToken), cancellationToken);
            UserAuthenticatedDto authenticatedDto = new(userDto, jwtToken, refreshToken.Token);
            return Result<UserAuthenticatedDto>.Success(authenticatedDto);
        }

        private static void IsValid(RefreshToken token)
        {
            if (token.DateExpires <= DateTime.UtcNow)
            {
                throw Result<RefreshToken>.Failure("token", "Token is expired", HttpStatusCode.BadRequest);
            }
        }
    }
}