using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IMongoClient client)
            : base(client.GetDatabase("Warehouse_users").GetCollection<User>("users"))
        {
        }
    }
}