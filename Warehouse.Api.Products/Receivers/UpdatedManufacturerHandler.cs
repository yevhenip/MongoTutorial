using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Receivers
{
    public class UpdatedManufacturerHandler : IConsumeAsync<UpdatedManufacturer>
    {
        private readonly IProductService _productService;

        public UpdatedManufacturerHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ConsumeAsync(UpdatedManufacturer message, CancellationToken cancellationToken = new())
        {
            await _productService.UpdateManufacturerInProductsAsync(message);
        }
    }
}