using System;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Users;
using MongoTutorial.Core.Settings;
using MongoTutorial.Data.Repositories;

namespace MongoTutorial.Api.Extensions
{
    public static class JwtExtensions
    {
        public static void AddJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtTokenConfiguration>(configuration.GetSection("JwtTokenConfiguration"));
            var jwtConfiguration = configuration.GetSection("JwtTokenConfiguration").Get<JwtTokenConfiguration>();

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userId = context.Principal?.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
                        var userRepository =
                            new UserRepository(new MongoClient(configuration["Data:ConnectionString"]));
                        var user = await userRepository.GetAsync(userId);
                        var sessionId = context.Principal?.Claims.SingleOrDefault(c => c.Type == "SessionId")?.Value;
                        if (user.SessionId != sessionId)
                        {
                            throw Result<UserAuthenticatedDto>.Failure("token", "This is invalid token",
                                HttpStatusCode.BadRequest);
                        }
                    }
                };
                o.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfiguration.Issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.Secret)),
                    ValidateAudience = true,
                    ValidAudience = jwtConfiguration.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        }
    }
}