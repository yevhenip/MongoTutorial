using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Api.Controllers.v1;
using MongoTutorial.Core.DTO.Users;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.User.Controllers.v1
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