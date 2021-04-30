using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Settings;
using Warehouse.Domain;

namespace Warehouse.Api.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IMongoClient client, IOptions<PollySettings> pollySettings)
            : base(client.GetDatabase("Warehouse_users").GetCollection<User>("users"), pollySettings.Value)
        {
        }
    }
}