using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;
using Warehouse.Domain;

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
        /// <returns>Authenticated user with bearer and refresh tokens</returns>
        Task<Result<UserAuthenticatedDto>> LoginAsync(LoginDto login);

        /// <summary>
        /// Refreshes user's session and sends message to update user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns>Authenticated user with bearer and refresh tokens</returns>
        Task<Result<UserAuthenticatedDto>> RefreshTokenAsync(string userId, TokenDto token);

        /// <summary>
        /// Invalidate user's session id and sends message to update user
        /// </summary>
        /// <param name="userId"></param>
        Task<Result<object>> LogoutAsync(string userId);

        Task DeleteTokenAsync(RefreshToken refreshToken);
    }
}