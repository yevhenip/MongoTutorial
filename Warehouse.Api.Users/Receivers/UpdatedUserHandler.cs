using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Users.Commands;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Api.Users.Receivers
{
    public class UpdatedUserHandler : IConsumeAsync<UpdatedUser>
    {
        private readonly IMediator _mediator;

        public UpdatedUserHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(UpdatedUser message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new UpdateUserCommandV2(message.User), cancellationToken);
        }
    }
}