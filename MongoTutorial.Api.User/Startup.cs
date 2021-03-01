using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MongoTutorial.Api.User.Business;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.User
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