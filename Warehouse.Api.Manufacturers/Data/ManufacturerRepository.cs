using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Manufacturers.Data
{
    public class ManufacturerRepository : IManufacturerRepository
    {
        private readonly IMongoCollection<Manufacturer> _manufacturerCollection;

        public ManufacturerRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse_manufacturers");
            _manufacturerCollection = db.GetCollection<Manufacturer>("manufacturers");
        }

        public Task<List<Manufacturer>> GetAllAsync()
        {
            return _manufacturerCollection.Find(_ => true).ToListAsync();
        }

        public Task<Manufacturer> GetAsync(string id)
        {
            return _manufacturerCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task CreateAsync(Manufacturer manufacturer)
        {
            return _manufacturerCollection.InsertOneAsync(manufacturer);
        }

        public Task UpdateAsync(Manufacturer manufacturer)
        {
            return _manufacturerCollection.ReplaceOneAsync(p => p.Id == manufacturer.Id, manufacturer);
        }

        public Task DeleteAsync(string id)
        {
            return _manufacturerCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }

        public Task<List<Manufacturer>> GetRangeAsync(IEnumerable<string> manufacturerIds)
        {
            return _manufacturerCollection.Find(m => manufacturerIds.Contains(m.Id)).ToListAsync();
        }
    }
}