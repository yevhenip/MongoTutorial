using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Api.Products.Receivers
{
    public class DeletedCustomerHandler : IConsumeAsync<DeletedCustomer>
    {
        private readonly IMediator _mediator;

        public DeletedCustomerHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(DeletedCustomer message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new DeleteCustomerFromProductCommand(message.Id), cancellationToken);
        }
    }
}