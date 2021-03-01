using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoTutorial.Core.Interfaces.Repositories;

namespace MongoTutorial.Api.Manufacturer.Data
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly IMongoCollection<Domain.Manufacturer> _manufacturerCollection;

        public ManufacturerRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse");
            _manufacturerCollection = db.GetCollection<Domain.Manufacturer>("manufacturers");
        }

        public Task<List<Domain.Manufacturer>> GetAllAsync()
        {
            return _manufacturerCollection.Find(_ => true).ToListAsync();
        }

        public Task<Domain.Manufacturer> GetAsync(string id)
        {
            return _manufacturerCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task CreateAsync(Domain.Manufacturer manufacturer)
        {
            return _manufacturerCollection.InsertOneAsync(manufacturer);
        }

        public Task UpdateAsync(Domain.Manufacturer manufacturer)
        {
            return _manufacturerCollection.ReplaceOneAsync(p => p.Id == manufacturer.Id, manufacturer);
        }

        public Task DeleteAsync(string id)
        {
            return _manufacturerCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }

        public Task<List<Domain.Manufacturer>> GetRangeAsync(IEnumerable<string> manufacturerIds)
        {
            return _manufacturerCollection.Find(m => manufacturerIds.Contains(m.Id)).ToListAsync();
        }
    }
}