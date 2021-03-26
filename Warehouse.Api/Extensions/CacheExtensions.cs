using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Warehouse.Core.Settings.CacheSettings;

namespace Warehouse.Api.Extensions
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Tries to get and deserialize data
        /// </summary>
        /// <param name="serializedCache"></param>
        /// <param name="cache"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Success of getting value</returns>
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

        /// <summary>
        /// Sets item to cache on cacheKey based on settings
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheKey"></param>
        /// <param name="item"></param>
        /// <param name="settings"></param>
        /// <typeparam name="T"></typeparam>
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

        /// <summary>
        /// Checks existence of cache
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheKey"></param>
        /// <returns>Success of operation</returns>
        public static async Task<bool> IsExistsAsync(this IDistributedCache distributedCache, string cacheKey)
        {
            var cache = await distributedCache.GetStringAsync(cacheKey);
            return !string.IsNullOrEmpty(cache);
        }

        /// <summary>
        /// Updates cache
        /// </summary>
        /// <param name="distributedCache"></param>
        /// <param name="cacheKey"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        public static async Task UpdateAsync<T>(this IDistributedCache distributedCache, string cacheKey, T item)
        {
            await distributedCache.RemoveAsync(cacheKey);
            var cache = JsonSerializer.Serialize(item);
            await distributedCache.SetStringAsync(cacheKey, cache);
        }
    }
}