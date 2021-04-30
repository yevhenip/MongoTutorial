using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Products.Receivers
{
    public class DeletedManufacturerHandler : ReceiverBase, IConsumeAsync<DeletedManufacturer>
    {
        public DeletedManufacturerHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(DeletedManufacturer message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new DeleteManufacturerFromProductCommand(message.Id), cancellationToken);
        }
    }
}