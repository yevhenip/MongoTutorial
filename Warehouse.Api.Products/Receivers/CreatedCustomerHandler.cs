using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedCustomerHandler : ReceiverBase, IConsumeAsync<CreatedCustomer>
    {
        public CreatedCustomerHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(CreatedCustomer message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new CreateCustomerCommand(message.Customer), cancellationToken);
        }
    }
}