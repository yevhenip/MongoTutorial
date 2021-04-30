using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Auth.Data;
using Warehouse.Api.Auth.Receivers;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using StartupBase = Warehouse.Api.Base.StartupBase;

namespace Warehouse.Api.Auth
{
    public class Startup : StartupBase
    {
        public Startup(IWebHostEnvironment environment) : base(environment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddSingleton<IPasswordHasher<UserDto>>(new PasswordHasher<UserDto>());
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<DeletedRefreshTokenHandler>();
            services.AddMediatR(typeof(Startup).Assembly);
        }

        protected override Assembly GetEventHandlerAssemblies()
        {
            return typeof(Startup).Assembly;
        }
    }
}