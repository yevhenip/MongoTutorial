using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Warehouse.Api.Common;

namespace Warehouse.Api.Extensions
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
                    var status = contextResponse.StatusCode;
                    string response;

                    if (!type.IsGenericType)
                    {
                        response = JsonSerializer.Serialize(new ApiError("unknown", result.Message, status));
                        contextResponse.StatusCode = status;
                        await contextResponse.WriteAsync(response);
                    }

                    var field = (string) type.GetProperty("Field")?.GetValue(result);
                    var error = (string) type.GetProperty("Message")?.GetValue(result);

                    status = (int) type.GetProperty("StatusCode").GetValue(result);
                    response = JsonSerializer.Serialize(new ApiError(field, error, status));
                    contextResponse.StatusCode = status;

                    await contextResponse.WriteAsync(response);
                });
            });
        }
    }
}