using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Base;
using Warehouse.Api.Users.Commands;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Api.Users.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    public class UsersController : ApiControllerBase
    {
        public UsersController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await Mediator.Send(new GetUsersCommand(_ => true));
            return Ok(result.Data);
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] string userId)
        {
            var result = await Mediator.Send(new GetUserCommand(userId));
            return Ok(result.Data);
        }


        [HttpGet("{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPageAsync([FromRoute] int page, [FromRoute] int pageSize)
        {
            var result = await Mediator.Send(new GetUsersPageCommand(page, pageSize));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Dungeon master")]
        [HttpPost("{userId:guid}")]
        public async Task<IActionResult> MakeAdminAsync([FromRoute] string userId)
        {
            var result = await Mediator.Send(new MakeAdminUserCommand(userId));
            return Ok(result.Data);
        }
        
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string userId, UserModelDto user)
        {
            var result = await Mediator.Send(new UpdateUserCommand(userId, user, UserName));
            return Ok(result.Data);
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string userId)
        {
            var result = await Mediator.Send(new DeleteUserCommand(userId));
            return Ok(result.Data);
        }
    }
}