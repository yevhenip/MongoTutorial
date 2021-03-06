using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Products.Business;
using Warehouse.Api.Products.Data;
using Warehouse.Api.Products.Messaging.Receiver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<IManufacturerRepository, ManufacturerRepository>();
            services.AddHostedService<ManufacturerDeleteReceiver>();
            services.AddHostedService<ManufacturerUpdateReceiver>();
            services.AddHostedService<ManufacturerCreateReceiver>();
            services.AddHostedService<CustomerDeleteReceiver>();
            services.AddHostedService<CustomerCreateReceiver>();
        }
    }
}