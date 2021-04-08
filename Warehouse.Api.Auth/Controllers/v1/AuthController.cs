﻿using System;
using System.Linq;
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

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            var result = await _authService.RegisterAsync(register);
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var sessionId = Guid.NewGuid().ToString();
            var result = await _authService.LoginAsync(login, sessionId);
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(TokenDto token)
        {
            var sessionId = Guid.NewGuid().ToString();
            var userId = User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
            var result = await _authService.RefreshTokenAsync(userId, token, sessionId);
            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task Logout()
        {
            var userId = User.Claims.SingleOrDefault(c => c.Type == "Id")?.Value;
            await _authService.LogoutAsync(userId);
        }
    }
}