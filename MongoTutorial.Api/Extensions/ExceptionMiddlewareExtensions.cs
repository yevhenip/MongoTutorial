using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MongoTutorial.Api.Common;

namespace MongoTutorial.Api.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseMyExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextResponse = context.Response;
                    contextResponse.ContentType = "application/json";
                    var result = context.Features.Get<IExceptionHandlerFeature>().Error;
                    var type = result.GetType();
                    string response;
                    var status = (HttpStatusCode) contextResponse.StatusCode;
                    if (!type.IsGenericType)
                    {
                        response = JsonSerializer.Serialize(new ApiError("unknown", result.Message, status));
                        contextResponse.StatusCode = (int) status;
                        await contextResponse.WriteAsync(response);
                    }

                    var field = (string) type.GetProperty("Field")?.GetValue(result);
                    var error = (string) type.GetProperty("Message")?.GetValue(result);
                    status = (HttpStatusCode) type.GetProperty("StatusCode").GetValue(result);
                    response = JsonSerializer.Serialize(new ApiError(field, error, status));
                    contextResponse.StatusCode = (int) status;
                    await contextResponse.WriteAsync(response);
                });
            });
        }
    }
}