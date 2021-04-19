using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets all users in database
        /// </summary>
        /// <returns>List of users</returns>
        Task<List<User>> GetAllAsync();


        public Task<List<User>> GetPageAsync(int page, int pageSize);

        public Task<long> GetCountAsync();

        /// <summary>
        /// Gets user in database based on provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        Task<User> GetAsync(string id);

        /// <summary>
        /// Gets user in database based on provided userName
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>User</returns>
        Task<User> GetByUserNameAsync(string userName);


        Task<User> GetByEmailAsync(string email);

        /// <summary>
        /// Gets users in database based on provided role
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns>List of users</returns>
        Task<List<User>> GetRangeByRoleAsync(string roleName);

        /// <summary>
        /// Sets new user to database
        /// </summary>
        /// <param name="user"></param>
        Task CreateAsync(User user);

        /// <summary>
        /// Updates user in database
        /// </summary>
        /// <param name="user"></param>
        Task UpdateAsync(User user);

        /// <summary>
        /// Deletes user from database
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(string id);
    }
}