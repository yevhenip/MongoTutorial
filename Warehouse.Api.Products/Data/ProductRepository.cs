using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Settings;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Data
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(IMongoClient client, IOptions<PollySettings> pollySettings)
            : base(client.GetDatabase("Warehouse_products").GetCollection<Product>("products"), pollySettings.Value)
        {
        }
    }
}