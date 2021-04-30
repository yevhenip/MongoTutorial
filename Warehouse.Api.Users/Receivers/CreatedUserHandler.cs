using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Users.Commands;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Api.Users.Receivers
{
    public class CreatedUserHandler : ReceiverBase,  IConsumeAsync<CreatedUser>
    {
        public CreatedUserHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(CreatedUser message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new CreateUserCommand(message.User));
        }
    }
}