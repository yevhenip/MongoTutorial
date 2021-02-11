using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Domain;

namespace MongoTutorial.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _productCollection;

        public ProductRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse");
            _productCollection = db.GetCollection<Product>("products");
        }

        public Task<List<Product>> GetProductsAsync()
        {
            return _productCollection.Find(_ => true).ToListAsync();
        }

        public Task<Product> GetProductByIdAsync(string id)
        {
            return _productCollection.Find(p => p.Id == id).SingleAsync();
        }
        
        public Task CreateProductAsync(Product product)
        {
            return _productCollection.InsertOneAsync(product);
        }

        public Task UpdateProductAsync(Product product)
        {
            return _productCollection.ReplaceOneAsync(p => p.Id == product.Id, product);
        }

        public Task DeleteProductAsync(string id)
        {
            return _productCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }
    }
}