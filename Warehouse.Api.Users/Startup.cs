using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Users.Business;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddScoped<IUserService, UserService>();
        }
    }
}