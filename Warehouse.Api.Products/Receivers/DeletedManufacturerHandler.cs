using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Products.Commands;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Products.Receivers
{
    public class DeletedManufacturerHandler : IConsumeAsync<DeletedManufacturer>
    {
        private readonly IMediator _mediator;

        public DeletedManufacturerHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(DeletedManufacturer message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new DeleteManufacturerFromProductCommand(message.Id), cancellationToken);
        }
    }
}