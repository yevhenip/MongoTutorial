using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Users;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<List<UserDto>>> GetAllAsync();
        
        Task<Result<PageDataDto<UserDto>>> GetPageAsync(int page, int pageSize);

        Task<Result<UserDto>> GetAsync(string id);

        Task<Result<UserDto>> GetByUserNameAsync(string userName);
        
        Task<Result<List<UserDto>>> GetRangeByRoleAsync(string roleName);

        Task<Result<UserDto>> CreateAsync(User user);

        Task<Result<UserDto>> UpdateAsync(string userId, UserModelDto user, string userName);
        
        Task UpdateAsync(User user);

        Task<Result<object>> DeleteAsync(string id);
    }
}