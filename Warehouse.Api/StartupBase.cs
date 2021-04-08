using System.IO;
using System.Text.Json;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using RabbitMQ.Client;
using Warehouse.Api.Business;
using Warehouse.Api.Extensions;
using Warehouse.Api.Common;
using Warehouse.Api.Data;
using Warehouse.Api.Messaging.Sender;
using Warehouse.Core.Interfaces.Messaging.Sender;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.MapperProfile.ProductProfile;
using Warehouse.Core.Settings.CacheSettings;

namespace Warehouse.Api
{
    public abstract class StartupBase
    {
        private IConfiguration Configuration { get; }
        private const string DefaultCors = "default";

        private IWebHostEnvironment Environment { get; }

        protected StartupBase(IWebHostEnvironment environment)
        {
            Environment = environment;
            var path = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Environment.ContentRootPath)
                .AddJsonFile(path + "/../Warehouse.Api/appsettings.json", true, true)
                .AddJsonFile(path + $"/../Warehouse.Api/appsettings.{Environment.EnvironmentName}.json", true);
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
                })
                .AddJsonOptions(opt =>
                    opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

            services.AddAutoMapper(typeof(ProductProfile).Assembly);

            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = Configuration["Cache:Configuration"];
                option.InstanceName = Configuration["Cache:InstanceName"];
            });

            services.AddSingleton<IMongoClient, MongoClient>(_ =>
                new MongoClient(Configuration["Data:ConnectionString"]));
            services.AddScoped<ValidateTokenSessionId>();
            services.AddScoped<ISender, Sender>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton(_ => new ConnectionFactory
            {
                HostName = Configuration["RabbitMq:HostName"],
                UserName = Configuration["RabbitMq:UserName"],
                Password = Configuration["RabbitMq:Password"]
            }.CreateConnection());

            services.Configure<CacheProductSettings>(Configuration.GetSection("Cache:CacheOptions:Product"));
            services.Configure<CacheManufacturerSettings>(Configuration.GetSection("Cache:CacheOptions:Manufacturer"));
            services.Configure<CacheUserSettings>(Configuration.GetSection("Cache:CacheOptions:User"));
            services.Configure<CacheCustomerSettings>(Configuration.GetSection("Cache:CacheOptions:Customer"));

            services.AddJwtBearerAuthentication(Configuration);
            
            services.AddCors(cors => cors.AddPolicy(DefaultCors,b =>
            {
                b.AllowAnyOrigin();
                b.AllowAnyHeader();
                b.AllowAnyMethod();
            }));
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
            app.UseCors(DefaultCors);
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}