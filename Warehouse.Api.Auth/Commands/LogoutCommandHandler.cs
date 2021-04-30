using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;
using ISender = Warehouse.Core.Interfaces.Services.ISender;

namespace Warehouse.Api.Auth.Commands
{
    public record LogoutCommand(string UserId) : IRequest<Result<object>>;

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<object>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ISender _sender;
        private readonly IRefreshTokenRepository _tokenRepository;

        public LogoutCommandHandler(IUserRepository userRepository, ISender sender,
            IRefreshTokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _sender = sender;
            _tokenRepository = tokenRepository;
        }

        public async Task<Result<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                throw Result<User>.Failure("id", "Invalid id");
            }

            user.SessionId = null;

            var token = await _tokenRepository.GetAsync(t => t.User.Id == request.UserId);
            await _tokenRepository.DeleteAsync(t => t.Id == token.Id);
            await _sender.PublishAsync(new UpdatedUser(user), cancellationToken);
            await _sender.PublishAsync(new DeletedToken(token.Id), cancellationToken);
            return Result<object>.Success();
        }
    }
}