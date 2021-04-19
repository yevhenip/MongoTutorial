using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Logs.Business;
using Warehouse.Api.Logs.Data;
using Warehouse.Api.Logs.Messaging.Receiver;
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
            services.AddSingleton<ILogRepository, LogRepository>();
            services.AddSingleton<ILogService, LogService>();
            services.AddHostedService<CreateLogReceiver>();
        }
    }
}