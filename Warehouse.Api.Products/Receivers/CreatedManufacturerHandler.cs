using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedManufacturerHandler : IConsumeAsync<CreatedManufacturer>
    {
        private readonly IMediator _mediator;

        public CreatedManufacturerHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(CreatedManufacturer message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new CreateManufacturerCommand(message.Manufacturer), cancellationToken);
        }
    }
}