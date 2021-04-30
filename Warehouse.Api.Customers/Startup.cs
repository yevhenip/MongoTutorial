using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Customers.Data;
using Warehouse.Core.Interfaces.Repositories;
using StartupBase = Warehouse.Api.Base.StartupBase;

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
            services.AddMediatR(typeof(Startup).Assembly);
        }
    }
}