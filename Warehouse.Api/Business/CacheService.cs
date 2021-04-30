using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings;
using Warehouse.Core.Settings.CacheSettings;

namespace Warehouse.Api.Business
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly AsyncCircuitBreakerPolicy _cachingPolicy;


        public CacheService(IDistributedCache cache, IOptions<PollySettings> pollySettings)
        {
            _cache = cache;
            _cachingPolicy = Policy.Handle<RedisConnectionException>()
                .Or<RedisTimeoutException>()
                .CircuitBreakerAsync(pollySettings.Value.RepeatedTimes,
                    TimeSpan.FromSeconds(pollySettings.Value.RepeatedDelay));
        }

        public Task<string> GetStringAsync(string cacheKey)
        {
            return _cachingPolicy.ExecuteAsync(() => _cache.GetStringAsync(cacheKey));
        }

        public async Task SetCacheAsync<T>(string cacheKey, T item, CacheBaseSettings settings)
        {
            var cache = JsonSerializer.Serialize(item);
            DistributedCacheEntryOptions cacheSettings = new()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(settings.AbsoluteExpiration),
                SlidingExpiration = TimeSpan.FromMinutes(settings.SlidingExpiration)
            };
            await _cachingPolicy.ExecuteAsync(() => _cache.SetStringAsync(cacheKey, cache, cacheSettings));
        }

        public async Task<bool> IsExistsAsync(string cacheKey)
        {
            var cache = await GetStringAsync(cacheKey);
            return !string.IsNullOrEmpty(cache);
        }

        public async Task UpdateAsync<T>(string cacheKey, T item, CacheBaseSettings settings)
        {
            await RemoveAsync(cacheKey);
            var cache = JsonSerializer.Serialize(item);
            await SetCacheAsync(cacheKey, cache, settings);
        }

        public Task RemoveAsync(string cacheKey)
        {
            return _cachingPolicy.ExecuteAsync(() => _cache.RemoveAsync(cacheKey));
        }
    }
}