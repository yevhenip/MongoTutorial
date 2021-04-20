using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Logs.Business;
using Warehouse.Api.Logs.Data;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;

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
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<CreatedLogHandler>();
        }

        protected override Assembly GetEventHandlerAssemblies()
        {
            return typeof(Startup).Assembly;
        }
    }
}