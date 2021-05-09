using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using ISender = Warehouse.Core.Interfaces.Services.ISender;

namespace Warehouse.Api.Users.Commands
{
    public record DeleteUserCommand(string UserId) : IRequest<Result<object>>;

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<object>>
    {
        private readonly ISender _sender;
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _tokenRepository;

        public DeleteUserCommandHandler(ISender sender, ICacheService cacheService, IUserRepository userRepository,
            IRefreshTokenRepository tokenRepository)
        {
            _sender = sender;
            _cacheService = cacheService;
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
        }

        public async Task<Result<object>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"User-{request.UserId}";
            if (!await _cacheService.IsExistsAsync(cacheKey))
            {
                var userInDb = await _userRepository.GetAsync(u => u.Id == request.UserId);
                userInDb.CheckForNull();
            }

            var token = await _tokenRepository.GetAsync(t => t.Id == request.UserId);
            if (token is not null)
            {
                await _tokenRepository.DeleteAsync(t => t.Id == token.Id);
                await _sender.PublishAsync(token, cancellationToken);
            }

            await _userRepository.DeleteAsync(u => u.Id == request.UserId);
            await _cacheService.RemoveAsync(cacheKey);

            return Result<object>.Success();
        }
    }
}