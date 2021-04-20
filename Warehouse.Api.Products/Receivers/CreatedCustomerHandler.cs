using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Receivers
{
    public class CreatedCustomerHandler : IConsumeAsync<Customer>
    {
        private readonly IProductService _productService;

        public CreatedCustomerHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ConsumeAsync(Customer message, CancellationToken cancellationToken = new())
        {
            await _productService.CreateCustomerAsync(message);
        }
    }
}