using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Api.Products.Receivers
{
    public class DeletedCustomerHandler : ReceiverBase, IConsumeAsync<DeletedCustomer>
    {
        public DeletedCustomerHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(DeletedCustomer message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new DeleteCustomerFromProductCommand(message.Id), cancellationToken);
        }
    }
}