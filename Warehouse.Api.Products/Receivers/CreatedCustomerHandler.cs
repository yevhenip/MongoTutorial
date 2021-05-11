using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedCustomerHandler : IConsumeAsync<CreatedCustomer>
    {
        private readonly IMediator _mediator;

        public CreatedCustomerHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(CreatedCustomer message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new CreateCustomerCommand(message.Customer), cancellationToken);
        }
    }
}