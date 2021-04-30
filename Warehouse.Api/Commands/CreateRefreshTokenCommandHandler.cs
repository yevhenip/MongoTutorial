using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Commands
{
    public record CreateRefreshTokenCommand(RefreshToken Token) : IRequest;

    public class CreateRefreshTokenCommandHandler : IRequestHandler<CreateRefreshTokenCommand>
    {
        private readonly IRefreshTokenRepository _tokenRepository;

        public CreateRefreshTokenCommandHandler(IRefreshTokenRepository tokenRepository = default)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<Unit> Handle(CreateRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            await _tokenRepository.CreateAsync(request.Token);
            return Unit.Value;
        }
    }
}