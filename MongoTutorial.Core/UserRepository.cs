using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoTutorial.Core.Interfaces.Repositories;

namespace MongoTutorial.Core
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<Domain.User> _userCollection;

        public UserRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse");
            _userCollection = db.GetCollection<Domain.User>("users");
        }

        public Task<List<Domain.User>> GetAllAsync()
        {
            return _userCollection.Find(_ => true).ToListAsync();
        }

        public Task<Domain.User> GetAsync(string id)
        {
            return _userCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task<Domain.User> GetByUserNameAsync(string userName)
        {
            return _userCollection.Find(p => p.UserName == userName).SingleOrDefaultAsync();
        }

        public Task<List<Domain.User>> GetRangeByRoleAsync(string roleName)
        {
            return _userCollection.Find(u => u.Roles.Any(r => r == roleName)).ToListAsync();
        }

        public Task CreateAsync(Domain.User user)
        {
            return _userCollection.InsertOneAsync(user);
        }

        public Task UpdateAsync(Domain.User user)
        {
            return _userCollection.ReplaceOneAsync(p => p.Id == user.Id, user);
        }

        public Task DeleteAsync(string id)
        {
            return _userCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }
    }
}