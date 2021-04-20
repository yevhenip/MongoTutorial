using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Products.Business;
using Warehouse.Api.Products.Data;
using Warehouse.Api.Products.Receivers;
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
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<CreatedManufacturerHandler>();
            services.AddScoped<DeletedManufacturerHandler>();
            services.AddScoped<UpdatedManufacturerHandler>();
            services.AddScoped<CreatedCustomerHandler>();
            services.AddScoped<DeletedCustomerHandler>();
        }

        protected override Assembly GetEventHandlerAssemblies()
        {
            return typeof(Startup).Assembly;
        }
    }
}