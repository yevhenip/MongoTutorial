using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Manufacturers.Data;
using Warehouse.Core.Interfaces.Repositories;
using StartupBase = Warehouse.Api.Base.StartupBase;

namespace Warehouse.Api.Manufacturers
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
            services.AddMediatR(typeof(Startup).Assembly);
        }
    }
}