using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Domain;

namespace MongoTutorial.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse");
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