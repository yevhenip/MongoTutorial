using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Users.Business;
using Warehouse.Api.Users.Data;
using Warehouse.Api.Users.Messaging.Receiver;
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
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddHostedService<UserUpdateReceiver>();
            services.AddHostedService<UserCreateReceiver>();
        }
    }
}