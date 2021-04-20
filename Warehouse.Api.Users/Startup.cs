using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Users.Business;
using Warehouse.Api.Users.Data;
using Warehouse.Api.Users.Receivers;
using Warehouse.Core.Interfaces.Repositories;
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
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<CreatedUserHandler>();
            services.AddScoped<UpdatedUserHandler>();
        }

        protected override Assembly GetEventHandlerAssemblies()
        {
            return typeof(Startup).Assembly;
        }
    }
}