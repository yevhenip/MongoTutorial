using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoTutorial.Api.Common;
using MongoTutorial.Api.Extensions;
using MongoTutorial.Business.Services;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.MapperProfile.ProductProfile;
using MongoTutorial.Data.Repositories;

namespace MongoTutorial.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation(o =>
                {
                    o.RegisterValidatorsFromAssembly(typeof(Startup).Assembly);
                    o.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });

            services.AddAutoMapper(typeof(ProductProfile).Assembly);

            services.Scan(sc =>
                sc.FromAssemblies(typeof(IProductRepository).Assembly, typeof(ProductRepository).Assembly,
                        typeof(ProductService).Assembly)
                    .AddClasses(@class =>
                        @class.Where(type =>
                            !type.Name.StartsWith('I')
                            && (type.Name.EndsWith("Repository") || type.Name.EndsWith("Service"))))
                    .AsImplementedInterfaces().WithScopedLifetime()
            );

            services.AddSingleton<IMongoClient, MongoClient>(_ =>
                new MongoClient(Configuration["Data:ConnectionString"]));
            services.AddScoped<ValidateTokenSessionId>();

            services.AddJwtBearerAuthentication(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMyExceptionHandler();
            app.UseHsts();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}