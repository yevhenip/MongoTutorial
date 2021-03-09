using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Extensions;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Messaging.Receiver
{
    public class ManufacturerUpdateReceiver : Receiver<Manufacturer>
    {
        private const string Queue = Queues.UpdateManufacturerQueue;
        private readonly IProductService _productService;


        public ManufacturerUpdateReceiver(IProductService productService, IConnection connection) : base(connection,
            Queue)
        {
            _productService = productService;
        }

        protected override async Task HandleMessage(Manufacturer manufacturer)
        {
            await _productService.UpdateManufacturerInProductsAsync(manufacturer);
        }
    }
}