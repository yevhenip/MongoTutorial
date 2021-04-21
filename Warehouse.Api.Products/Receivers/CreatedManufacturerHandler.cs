using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedManufacturerHandler : IConsumeAsync<CreatedManufacturer>
    {
        private readonly IProductService _productService;

        public CreatedManufacturerHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ConsumeAsync(CreatedManufacturer message, CancellationToken cancellationToken = new())
        {
            await _productService.CreateManufacturerAsync(message);
        }
    }
}