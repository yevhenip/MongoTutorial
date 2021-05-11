using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Commands;
using Warehouse.Core.DTO.Auth;

namespace Warehouse.Api.Users.Receivers
{
    public class CreatedRefreshTokenHandler : IConsumeAsync<CreatedToken>
    {
        private readonly IMediator _mediator;

        public CreatedRefreshTokenHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(CreatedToken message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new CreateRefreshTokenCommand(message.Token), cancellationToken);
        }
    }
}