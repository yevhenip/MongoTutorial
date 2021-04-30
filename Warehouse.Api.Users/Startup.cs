using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Api.Users.Data;
using Warehouse.Api.Users.Receivers;
using Warehouse.Core.Interfaces.Repositories;
using StartupBase = Warehouse.Api.Base.StartupBase;

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
            services.AddScoped<DeletedRefreshTokenHandler>();
            services.AddScoped<CreatedRefreshTokenHandler>();
            services.AddScoped<CreatedUserHandler>();
            services.AddScoped<UpdatedUserHandler>();
            services.AddMediatR(typeof(Startup).Assembly);
        }

        protected override Assembly GetEventHandlerAssemblies()
        {
            return typeof(Startup).Assembly;
        }
    }
}