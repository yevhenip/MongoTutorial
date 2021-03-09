using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Products");
            _productCollection = db.GetCollection<Product>("products");
        }

        public Task<List<Product>> GetAllAsync()
        {
            return _productCollection.Find(_ => true).ToListAsync();
        }

        public Task<Product> GetAsync(string id)
        {
            return _productCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task CreateAsync(Product product)
        {
            return _productCollection.InsertOneAsync(product);
        }

        public Task UpdateAsync(Product product)
        {
            return _productCollection.ReplaceOneAsync(p => p.Id == product.Id, product);
        }

        public Task DeleteAsync(string id)
        {
            return _productCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }

        public Task<List<Product>> GetRangeByManufacturerId(string manufacturerId)
        {
            return _productCollection.Find(p =>
                p.Manufacturers.Any(m => m.Id == manufacturerId)).ToListAsync();
        }

        public Task<Product> GetByCustomerId(string customerId)
        {
            return _productCollection.FindSync(p => p.Customer.Id == customerId).SingleOrDefaultAsync();
        }
    }
}