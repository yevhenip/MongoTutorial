using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Receivers
{
    public class DeletedCustomerHandler : IConsumeAsync<string>
    {
        private readonly IProductService _productService;

        public DeletedCustomerHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task ConsumeAsync(string message, CancellationToken cancellationToken = new())
        {
            await _productService.DeleteCustomerFromProductAsync(message);
        }
    }
}