using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Registers user and sends message to store user in database
        /// </summary>
        /// <param name="register"></param>
        /// <returns>Created user</returns>
        Task<Result<UserAuthenticatedDto>> RegisterAsync(RegisterDto register);

        /// <summary>
        /// Logins user and sends message to update user
        /// </summary>
        /// <param name="login"></param>
        /// <param name="sessionId"></param>
        /// <returns>Authenticated user with bearer and refresh tokens</returns>
        Task<Result<UserAuthenticatedDto>> LoginAsync(LoginDto login, string sessionId);

        /// <summary>
        /// Refreshes user's session and sends message to update user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="sessionId"></param>
        /// <returns>Authenticated user with bearer and refresh tokens</returns>
        Task<Result<UserAuthenticatedDto>> RefreshTokenAsync(string userId, TokenDto token, string sessionId);

        /// <summary>
        /// Invalidate user's session id and sends message to update user
        /// </summary>
        /// <param name="userId"></param>
        Task<Result<object>> LogoutAsync(string userId);
    }
}