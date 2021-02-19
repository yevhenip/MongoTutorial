using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Auth;
using MongoTutorial.Core.DTO.Users;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.Controllers.v1
{
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            var result = await _authService.RegisterAsync(register);
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            HttpContext.Session.Set("What", new byte[] {1, 2, 3, 4, 5});
            var sessionId = HttpContext.Session.Id;
            var result = await _authService.LoginAsync(login, sessionId);
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(TokenDto token)
        {
            var sessionId = HttpContext.Session.Id;
            if (sessionId == User.Claims.SingleOrDefault(c => c.Type == "SessionId")?.Value)
            {
                throw Result<UserAuthenticatedDto>.Failure("session", "Session is actual");
            }

            var userId = User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
            var result = await _authService.RefreshTokenAsync(userId, token, sessionId);
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task Logout()
        {
            var userId = User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
            await _authService.LogoutAsync(userId);
        }
    }
}