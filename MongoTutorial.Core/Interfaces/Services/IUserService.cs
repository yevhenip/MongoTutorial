using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Users;

namespace MongoTutorial.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<List<UserDto>>> GetAllAsync();
        
        Task<Result<UserDto>> GetAsync(string id);

        Task<Result<UserDto>> GetByUserNameAsync(string userName);
        
        Task<Result<List<UserDto>>> GetRangeByRoleAsync(string roleName);

        Task<Result<UserDto>> CreateAsync(UserDto user);

        Task<Result<UserDto>> UpdateAsync(string userId, UserModelDto user);
        
        Task<Result<UserDto>> UpdateAsync(UserDto user);

        Task<Result<object>> DeleteAsync(string id);
    }
}