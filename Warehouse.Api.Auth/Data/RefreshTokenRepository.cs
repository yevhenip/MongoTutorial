using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Auth.Data
{
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(IMongoClient client)
            : base(client.GetDatabase("Warehouse_auth").GetCollection<RefreshToken>("refreshTokens"))
        {
        }
    }
}