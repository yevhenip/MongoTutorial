using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Products.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Domain.Product> _productCollection;

        public ProductRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse");
            _productCollection = db.GetCollection<Domain.Product>("products");
        }

        public Task<List<Domain.Product>> GetAllAsync()
        {
            return _productCollection.Find(_ => true).ToListAsync();
        }

        public Task<Domain.Product> GetAsync(string id)
        {
            return _productCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task CreateAsync(Domain.Product product)
        {
            return _productCollection.InsertOneAsync(product);
        }

        public Task UpdateAsync(Domain.Product product)
        {
            return _productCollection.ReplaceOneAsync(p => p.Id == product.Id, product);
        }

        public Task DeleteAsync(string id)
        {
            return _productCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }

        public Task<List<Domain.Product>> GetRangeByManufacturerId(string manufacturerId)
        {
            return _productCollection.Find(p =>
                p.Manufacturers.Any(m => m.Id == manufacturerId)).ToListAsync();
        }
    }
}