using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedManufacturerHandler : IConsumeAsync<Manufacturer>
    {
        private readonly IProductService _productService;

        public CreatedManufacturerHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ConsumeAsync(Manufacturer message, CancellationToken cancellationToken = new())
        {
            await _productService.CreateManufacturerAsync(message);
        }
    }
}