using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Commands
{
    public record UpdateManufacturerInProductCommand(Manufacturer Manufacturer) : IRequest;

    public class UpdateManufacturerInProductCommandHandler : IRequestHandler<UpdateManufacturerInProductCommand>
    {
        private readonly IProductRepository _productRepository;
        private readonly IManufacturerRepository _manufacturerRepository;

        public UpdateManufacturerInProductCommandHandler(IProductRepository productRepository,
            IManufacturerRepository manufacturerRepository)
        {
            _productRepository = productRepository;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Unit> Handle(UpdateManufacturerInProductCommand request, CancellationToken cancellationToken)
        {
            var products = await _productRepository.GetRangeAsync(p =>
                p.Manufacturers.Any(m => m.Id == request.Manufacturer.Id));
            foreach (var product in products)
            {
                await UpdateManufacturerInProductAsync(product, request.Manufacturer.Id,
                    request.Manufacturer);
            }

            await _manufacturerRepository.UpdateAsync(
                m => m.Id == request.Manufacturer.Id, request.Manufacturer);
            return Unit.Value;
        }

        private async Task UpdateManufacturerInProductAsync(Product product, string manufacturerId,
            Manufacturer manufacturer)
        {
            var manufacturers = product.Manufacturers.ToList();
            manufacturers.RemoveAll(m => m.Id == manufacturerId);
            manufacturers.Add(manufacturer);
            product.Manufacturers = manufacturers;
            await _productRepository.UpdateAsync(p => p.Id == product.Id, product);
        }
    }
}