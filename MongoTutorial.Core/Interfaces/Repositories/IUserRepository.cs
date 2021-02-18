using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Domain;

namespace MongoTutorial.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        
        Task<User> GetAsync(string id);

        Task<User> GetByUserNameAsync(string userName);
        
        Task<List<User>> GetRangeByRoleAsync(string roleName);

        Task CreateAsync(User user);

        Task UpdateAsync(User user);

        Task DeleteAsync(string id);
    }
}