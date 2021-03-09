using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Extensions;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Messaging.Receiver
{
    public class ManufacturerDeleteReceiver : Receiver<string>
    {
        private const string Queue = Queues.DeleteManufacturerQueue;
        private readonly IProductService _productService;


        public ManufacturerDeleteReceiver(IProductService productService, IConnection connection) : base(connection,
            Queue)
        {
            _productService = productService;
        }

        protected override async Task HandleMessage(string manufacturerId)
        {
            await _productService.DeleteManufacturerFromProductAsync(manufacturerId);
        }
    }
}