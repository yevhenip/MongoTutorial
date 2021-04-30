using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Logs.Data;
using Warehouse.Api.Logs.Receivers;
using Warehouse.Core.Interfaces.Repositories;
using StartupBase = Warehouse.Api.Base.StartupBase;

namespace Warehouse.Api.Logs
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<CreatedLogHandler>();
            services.AddMediatR(typeof(Startup).Assembly);
        }

        protected override Assembly GetEventHandlerAssemblies()
        {
            return typeof(Startup).Assembly;
        }
    }
}