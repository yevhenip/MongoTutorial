using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Commands;
using Warehouse.Domain;

namespace Warehouse.Api.Auth.Receivers
{
    public class DeletedRefreshTokenHandler : IConsumeAsync<RefreshToken>
    {
        private readonly IMediator _mediator;

        public DeletedRefreshTokenHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(RefreshToken message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new DeleteRefreshTokenCommand(message.Id), cancellationToken);
        }
    }
}