using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Controllers.v1;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Auth.Controllers.v1
{
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers user
        /// </summary>
        /// <param name="register"></param>
        /// <returns>Created user</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            var result = await _authService.RegisterAsync(register);
            return Ok(result.Data);
        }

        /// <summary>
        /// Logins user
        /// </summary>
        /// <param name="login"></param>
        /// <returns>Authenticated user with bearer and refresh tokens</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var sessionId = Guid.NewGuid().ToString();
            var result = await _authService.LoginAsync(login, sessionId);
            return Ok(result.Data);
        }

        /// <summary>
        /// Refreshes user's session
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Authenticated user with bearer and refresh tokens</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(TokenDto token)
        {
            var sessionId = Guid.NewGuid().ToString();
            var result = await _authService.RefreshTokenAsync(Id, token, sessionId);
            return Ok(result.Data);
        }

        /// <summary>
        /// Logouts user invalidating bearer token
        /// </summary>
        [Authorize]
        [HttpPost("[action]")]
        public async Task Logout()
        {
            await _authService.LogoutAsync(Id);
        }
    }
}