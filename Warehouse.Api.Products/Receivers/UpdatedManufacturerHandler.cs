using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Products.Receivers
{
    public class UpdatedManufacturerHandler : IConsumeAsync<UpdatedManufacturer>
    {
        private readonly IMediator _mediator;

        public UpdatedManufacturerHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(UpdatedManufacturer message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new UpdateManufacturerInProductCommand(message.Manufacturer), cancellationToken);
        }
    }
}