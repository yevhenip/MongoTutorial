using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Data
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(IMongoClient client)
            : base(client.GetDatabase("Warehouse_products").GetCollection<Product>("products"))
        {
        }
    }
}