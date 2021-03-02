using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Warehouse.Core.Settings.CacheSettings;

namespace Warehouse.Core.Extensions
{
    public static class CacheExtensions
    {
        public static bool TryGetValue<T>(this string serializedCache, out T cache)
        {
            cache = default;
            if (serializedCache == null)
            {
                return false;
            }

            cache = (T) JsonSerializer.Deserialize(serializedCache, typeof(T));
            return true;
        }

        public static async Task SetCacheAsync<T>(this IDistributedCache distributedCache, string cacheKey, T item,
            CacheBaseSettings settings)
        {
            var cache = JsonSerializer.Serialize(item);
            DistributedCacheEntryOptions cacheSettings = new()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(settings.AbsoluteExpiration),
                SlidingExpiration = TimeSpan.FromMinutes(settings.SlidingExpiration)
            };
            await distributedCache.SetStringAsync(cacheKey, cache, cacheSettings);
        }

        public static async Task<bool> IsExistsAsync(this IDistributedCache distributedCache, string cacheKey)
        {
            var cache = await distributedCache.GetStringAsync(cacheKey);
            return cache is not null;
        }

        public static async Task UpdateAsync<T>(this IDistributedCache distributedCache, string cacheKey, T item)
        {
            await distributedCache.RemoveAsync(cacheKey);
            var cache = JsonSerializer.Serialize(item);
            await distributedCache.SetStringAsync(cacheKey, cache);
        }
    }
}