using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Controllers.v1;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users.Controllers.v1
{
    [Authorize(Roles = "Admin")]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result.Data);
        }
        
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string userId)
        {
            var result = await _userService.GetAsync(userId);
            return Ok(result.Data);
        }
        
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string userId, UserModelDto user)
        {
            var result = await _userService.UpdateAsync(userId, user);
            return Ok(result.Data);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string userId)
        {
            var result = await _userService.DeleteAsync(userId);
            return Ok(result.Data);
        }
    }
}