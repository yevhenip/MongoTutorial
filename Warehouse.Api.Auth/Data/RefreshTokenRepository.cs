using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Auth.Data
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IMongoCollection<RefreshToken> _tokenCollection;

        public RefreshTokenRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Auth");
            _tokenCollection = db.GetCollection<RefreshToken>("refreshTokens");
        }

        public Task<List<RefreshToken>> GetAllAsync()
        {
            return _tokenCollection.Find(_ => true).ToListAsync();
        }

        public Task<RefreshToken> GetAsync(string userId, string token)
        {
            return _tokenCollection.Find(p => p.User.Id == userId && p.Token == token).SingleOrDefaultAsync();
        }

        public Task<RefreshToken> GetByUserIdAsync(string userId)
            {
            return _tokenCollection.Find(p => p.User.Id == userId).SingleOrDefaultAsync();
        }
        
        public Task CreateAsync(RefreshToken token)
        {
            return _tokenCollection.InsertOneAsync(token);
        }

        public Task UpdateAsync(RefreshToken token)
        {
            return _tokenCollection.ReplaceOneAsync(p => p.Id == token.Id, token);
        }

        public Task DeleteAsync(string id)
        {
            return _tokenCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }
    }
}