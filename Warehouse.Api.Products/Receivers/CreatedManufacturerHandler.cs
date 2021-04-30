using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedManufacturerHandler : ReceiverBase, IConsumeAsync<CreatedManufacturer>
    {
        public CreatedManufacturerHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(CreatedManufacturer message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new CreateManufacturerCommand(message.Manufacturer), cancellationToken);
        }
    }
}