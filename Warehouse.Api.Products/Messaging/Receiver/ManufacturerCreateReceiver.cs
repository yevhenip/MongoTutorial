using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Messaging.Receiver
{
    public class ManufacturerCreateReceiver : Receiver<Manufacturer>
    {
        private const string Queue = "CreateManufacturerQueue";
        private readonly IProductService _productService;
        
        public ManufacturerCreateReceiver(IProductService productService, IConnection connection) : base(connection,
            Queue)
        {
            _productService = productService;
        }

        protected override async Task HandleMessage(Manufacturer manufacturer)
        {
            await _productService.CreateManufacturerAsync(manufacturer);
        }
    }
}