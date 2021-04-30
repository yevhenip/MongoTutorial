using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Commands
{
    public record CreateManufacturerCommand(Manufacturer Manufacturer) : IRequest;

    public class CreateManufacturerCommandHandler : IRequestHandler<CreateManufacturerCommand>
    {
        private readonly IManufacturerRepository _manufacturerRepository;

        public CreateManufacturerCommandHandler(IManufacturerRepository manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
        }

        public Task<Unit> Handle(CreateManufacturerCommand request, CancellationToken cancellationToken)
        {
            _manufacturerRepository.CreateAsync(request.Manufacturer);
            return Unit.Task;
        }
    }
}