using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Extensions;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Messaging.Receiver
{
    public class CustomerDeleteReceiver: Receiver<string>
    {
        private const string Queue = Queues.DeleteCustomerQueue;
        private readonly IProductService _productService;


        public CustomerDeleteReceiver(IProductService productService, IConnection connection) : base(connection, Queue)
        {
            _productService = productService;
        }
        
        protected override async Task HandleMessage(string customerId)
        {
            await _productService.DeleteCustomerFromProductAsync(customerId);
        }
    }
}