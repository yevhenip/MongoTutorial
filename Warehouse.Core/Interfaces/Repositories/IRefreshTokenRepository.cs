using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<List<RefreshToken>> GetAllAsync();

        Task<RefreshToken> GetAsync(string userId, string token);

        Task<RefreshToken> GetByUserIdAsync(string userId);
        
        Task CreateAsync(RefreshToken token);

        Task UpdateAsync(RefreshToken token);

        Task DeleteAsync(string id);
    }
}