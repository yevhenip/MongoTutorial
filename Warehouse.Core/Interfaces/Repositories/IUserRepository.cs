using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        public Task<List<User>> GetPageAsync(int page, int pageSize);

        public Task<long> GetCountAsync();

        Task<User> GetAsync(string id);

        Task<User> GetByUserNameAsync(string userName);

        Task<User> GetByEmailAsync(string email);

        Task<List<User>> GetRangeByRoleAsync(string roleName);

        Task CreateAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(string id);
    }
}