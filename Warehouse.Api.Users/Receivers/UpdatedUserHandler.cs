using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Users.Commands;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Api.Users.Receivers
{
    public class UpdatedUserHandler : ReceiverBase, IConsumeAsync<UpdatedUser>
    {
        public UpdatedUserHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(UpdatedUser message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new UpdateUserCommandV2(message.User), cancellationToken);
        }
    }
}