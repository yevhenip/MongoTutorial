using System.Text.Json;

namespace Warehouse.Api.Extensions
{
    public static class CacheExtensions
    {
        public static bool TryGetValue<T>(this string serializedCache, out T cache)
        {
            cache = default;
            if (string.IsNullOrEmpty(serializedCache))
            {
                return false;
            }

            cache = (T) JsonSerializer.Deserialize(serializedCache, typeof(T));
            return true;
        }
    }
}