using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Commands;
using Warehouse.Core.DTO.Auth;

namespace Warehouse.Api.Users.Receivers
{
    public class CreatedRefreshTokenHandler : ReceiverBase, IConsumeAsync<CreatedToken>
    {
        public CreatedRefreshTokenHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(CreatedToken message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new CreateRefreshTokenCommand(message.Token), cancellationToken);
        }
    }
}