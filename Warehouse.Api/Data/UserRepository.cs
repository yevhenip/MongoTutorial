using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse_users");
            _userCollection = db.GetCollection<User>("users");
        }

        public Task<List<User>> GetAllAsync()
        {
            return _userCollection.Find(_ => true).ToListAsync();
        }

        public Task<User> GetAsync(string id)
        {
            return _userCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task<User> GetByUserNameAsync(string userName)
        {
            return _userCollection.Find(p => p.UserName == userName).SingleOrDefaultAsync();
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return _userCollection.Find(p => p.Email == email).SingleOrDefaultAsync();
        }

        public Task<List<User>> GetRangeByRoleAsync(string roleName)
        {
            return _userCollection.Find(u => u.Roles.Any(r => r == roleName)).ToListAsync();
        }

        public Task CreateAsync(User user)
        {
            return _userCollection.InsertOneAsync(user);
        }

        public Task UpdateAsync(User user)
        {
            return _userCollection.ReplaceOneAsync(p => p.Id == user.Id, user);
        }

        public Task DeleteAsync(string id)
        {
            return _userCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }
    }
}