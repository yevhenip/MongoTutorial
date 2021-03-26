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
        
        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result.Data);
        }
        
        /// <summary>
        /// Gets user based on provided id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>User</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string userId)
        {
            var result = await _userService.GetAsync(userId);
            return Ok(result.Data);
        }
        
        /// <summary>
        /// Updates product
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="user"></param>
        /// <returns>Updated user</returns>
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string userId, UserModelDto user)
        {
            var result = await _userService.UpdateAsync(userId, user);
            return Ok(result.Data);
        }

        /// <summary>
        /// Deletes user
        /// </summary>
        /// <param name="userId"></param>
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string userId)
        {
            var result = await _userService.DeleteAsync(userId);
            return Ok(result.Data);
        }
    }
}