using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Core.DTO.Auth;
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
            var user = (await _authService.RegisterAsync(register)).Data;
            return Ok(user);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var result = await _authService.LoginAsync(login);
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(TokenDto token)
        {
            var userId = User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
            var result = await _authService.RefreshTokenAsync(userId, token);
            return Ok(result.Data);
        }
        
        [HttpPost("[action]")]
        public IActionResult ConfirmEmail(string userid,string token)
        {
            return Ok();
        }
    }
}