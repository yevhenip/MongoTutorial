using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Customers.Business;
using Warehouse.Api.Customers.Data;
using Warehouse.Api.Messaging.Sender;
using Warehouse.Core.Interfaces.Messaging.Sender;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Customers
{
    public class Startup : StartupBase
    {
        
        public Startup(IWebHostEnvironment environment) : base(environment)
        {
        }
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ISender, Sender>();
        }
    }
}