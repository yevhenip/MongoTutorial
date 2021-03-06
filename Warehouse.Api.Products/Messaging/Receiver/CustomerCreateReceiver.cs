using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Messaging.Receiver
{
    public class CustomerCreateReceiver: Receiver<Customer>
    {
        private const string Queue = "CreateCustomerQueue";
        private readonly IProductService _productService;


        public CustomerCreateReceiver(IProductService productService, IConnection connection) : base(connection, Queue)
        {
            _productService = productService;
        }

        protected override async Task HandleMessage(Customer customer)
        {
            await _productService.CreateCustomerAsync(customer);
        }
    }
}