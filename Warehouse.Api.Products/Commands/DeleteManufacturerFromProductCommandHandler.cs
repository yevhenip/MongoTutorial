using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Products.Commands
{
    public record DeleteManufacturerFromProductCommand(string Id) : IRequest;

    public class DeleteManufacturerFromProductCommandHandler : IRequestHandler<DeleteManufacturerFromProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IManufacturerRepository _manufacturerRepository;

        public DeleteManufacturerFromProductCommandHandler(IProductRepository productRepository,
            IManufacturerRepository manufacturerRepository)
        {
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Unit> Handle(DeleteManufacturerFromProductCommand request,
            CancellationToken cancellationToken)
        {
            var products =
                await _productRepository.GetRangeAsync(p => p.Manufacturers.Any(m => m.Id == request.Id));
            foreach (var product in products)
            {
                product.Manufacturers.RemoveAll(m => m.Id == request.Id);
                await _productRepository.UpdateAsync(p => p.Id == product.Id, product);
            }

            await _manufacturerRepository.DeleteAsync(m => m.Id == request.Id);
            return Unit.Value;
        }
    }
}