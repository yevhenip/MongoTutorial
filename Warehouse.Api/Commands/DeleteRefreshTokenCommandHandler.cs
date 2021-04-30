using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Commands
{
    public record DeleteRefreshTokenCommand(string Id) : IRequest;

    public class DeleteRefreshTokenCommandHandler : IRequestHandler<DeleteRefreshTokenCommand>
    {
        private readonly IRefreshTokenRepository _tokenRepository;

        public DeleteRefreshTokenCommandHandler(IRefreshTokenRepository tokenRepository = default)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<Unit> Handle(DeleteRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            await _tokenRepository.DeleteAsync(t => t.Id == request.Id);
            return Unit.Value;
        }
    }
}