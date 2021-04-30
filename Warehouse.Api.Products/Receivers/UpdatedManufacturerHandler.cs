using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Products.Receivers
{
    public class UpdatedManufacturerHandler : ReceiverBase, IConsumeAsync<UpdatedManufacturer>
    {
        public UpdatedManufacturerHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(UpdatedManufacturer message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new UpdateManufacturerInProductCommand(message.Manufacturer), cancellationToken);
        }
    }
}