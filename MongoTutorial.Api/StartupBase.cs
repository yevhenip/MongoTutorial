using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MongoTutorial.Api.Common;
using MongoTutorial.Api.Extensions;
using MongoTutorial.Core;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.MapperProfile.ProductProfile;
using MongoTutorial.Core.Settings.CacheSettings;

namespace MongoTutorial.Api
{
    public abstract class StartupBase
    {
        protected virtual IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        protected StartupBase(IWebHostEnvironment environment)
        {
            Environment = environment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.ContentRootPath)
                .AddJsonFile("/appsettings.json", true, true)
                .AddJsonFile($"/appsettings.{Environment.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddFluentValidation(o =>
                {
                    o.RegisterValidatorsFromAssembly(typeof(StartupBase).Assembly);
                    o.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });

            services.AddAutoMapper(typeof(ProductProfile).Assembly);

            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = Configuration["Cache:Configuration"];
                option.InstanceName = Configuration["Cache:InstanceName"];
            });

            services.AddSingleton<IMongoClient, MongoClient>(_ =>
                new MongoClient(Configuration["Data:ConnectionString"]));
            services.AddScoped<ValidateTokenSessionId>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.Configure<CacheProductSettings>(Configuration.GetSection("Cache:CacheOptions:Product"));
            services.Configure<CacheManufacturerSettings>(Configuration.GetSection("Cache:CacheOptions:Manufacturer"));
            services.Configure<CacheUserSettings>(Configuration.GetSection("Cache:CacheOptions:User"));

            services.AddJwtBearerAuthentication(Configuration);
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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