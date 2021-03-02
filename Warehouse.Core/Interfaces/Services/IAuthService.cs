using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<UserDto>> RegisterAsync(RegisterDto register);
        Task<Result<UserAuthenticatedDto>> LoginAsync(LoginDto login, string sessionId);
        Task<Result<UserAuthenticatedDto>> RefreshTokenAsync(string userId, TokenDto token, string sessionId);
        Task<Result<object>> LogoutAsync(string userId);
    }
}