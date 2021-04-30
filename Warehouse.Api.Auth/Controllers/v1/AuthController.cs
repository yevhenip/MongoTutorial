using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Auth.Commands;
using Warehouse.Api.Base;
using Warehouse.Core.DTO.Auth;

namespace Warehouse.Api.Auth.Controllers.v1
{
    public class AuthController : ApiControllerBase
    {
        public AuthController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register(RegisterDto register)
        {
            var result = await Mediator.Send(new RegisterCommand(register));
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var result = await Mediator.Send(new LoginCommand(login));
            return Ok(result.Data);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken(TokenDto token)
        {
            var result = await Mediator.Send(new RefreshTokenCommand(Id, token));
            return Ok(result.Data);
        }

        [Authorize]
        [HttpPost("[action]")]
        public async Task Logout()
        {
            await Mediator.Send(new LogoutCommand(Id));
        }
    }
}