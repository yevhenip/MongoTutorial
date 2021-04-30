using System.Threading.Tasks;
using Warehouse.Core.Settings.CacheSettings;

namespace Warehouse.Core.Interfaces.Services
{
    public interface ICacheService
    {
        Task<string> GetStringAsync(string cacheKey);

        Task SetCacheAsync<T>(string cacheKey, T item, CacheBaseSettings settings);

        Task<bool> IsExistsAsync(string cacheKey);

        Task UpdateAsync<T>(string cacheKey, T item, CacheBaseSettings settings);

        Task RemoveAsync(string cacheKey);
    }
}