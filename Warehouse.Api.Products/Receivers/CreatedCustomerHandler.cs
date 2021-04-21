using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedCustomerHandler : IConsumeAsync<CreatedCustomer>
    {
        private readonly IProductService _productService;

        public CreatedCustomerHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ConsumeAsync(CreatedCustomer message, CancellationToken cancellationToken = new())
        {
            await _productService.CreateCustomerAsync(message);
        }
    }
}