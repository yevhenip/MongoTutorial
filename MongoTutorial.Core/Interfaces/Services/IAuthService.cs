using System.Threading.Tasks;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Auth;
using MongoTutorial.Core.DTO.Users;

namespace MongoTutorial.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<UserDto>> RegisterAsync(RegisterDto register);
        Task<Result<UserAuthenticatedDto>> LoginAsync(LoginDto login);
        Task<Result<UserAuthenticatedDto>> RefreshTokenAsync(string userId, TokenDto token);
    }
}