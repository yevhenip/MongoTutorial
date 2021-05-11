using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Users.Commands;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Api.Users.Receivers
{
    public class CreatedUserHandler : IConsumeAsync<CreatedUser>
    {
        private readonly IMediator _mediator;

        public CreatedUserHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(CreatedUser message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new CreateUserCommand(message.User), cancellationToken);
        }
    }
}