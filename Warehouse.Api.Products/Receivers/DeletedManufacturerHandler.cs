using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Receivers
{
    public class DeletedManufacturerHandler : IConsumeAsync<DeletedManufacturer>
    {
        private readonly IProductService _productService;

        public DeletedManufacturerHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ConsumeAsync(DeletedManufacturer message, CancellationToken cancellationToken = new())
        {
            await _productService.DeleteManufacturerFromProductAsync(message);
        }
    }
}