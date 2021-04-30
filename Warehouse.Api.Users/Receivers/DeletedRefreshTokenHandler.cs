using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Commands;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Receivers
{
    public class DeletedRefreshTokenHandler : ReceiverBase, IConsumeAsync<RefreshToken>
    {
        public DeletedRefreshTokenHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(RefreshToken message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new DeleteRefreshTokenCommand(message.Id), cancellationToken);
        }
    }
}