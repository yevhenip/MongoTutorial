using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        /// <summary>
        /// Gets refresh token in database based on user id and token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns>Refresh token</returns>
        Task<RefreshToken> GetAsync(string userId, string token);

        /// <summary>
        ///  Gets refresh token in database based on user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Refresh token</returns>
        Task<RefreshToken> GetByUserIdAsync(string userId);
        
        /// <summary>
        /// Sets new refresh token to database
        /// </summary>
        /// <param name="token"></param>
        Task CreateAsync(RefreshToken token);

        /// <summary>
        /// Updates refresh token in database
        /// </summary>
        /// <param name="token"></param>
        Task UpdateAsync(RefreshToken token);

        /// <summary>
        /// Deletes refresh token from database
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(string id);
    }
}