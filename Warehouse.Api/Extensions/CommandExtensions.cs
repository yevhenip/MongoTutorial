using System.Net;
using System.Text.Json;
using Warehouse.Api.Common;

namespace Warehouse.Api.Extensions
{
    public static class CommandExtensions
    {
        public static JsonSerializerOptions JsonSerializerOptions =>
            new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};
        
        public static void CheckForNull<T>(this T item)
        {
            if (item == null)
            {
                var typeName = typeof(T).Name;
                throw Result<T>.Failure("id", $"{typeName} doesn't exists", HttpStatusCode.BadRequest);
            }
        }
    }
}