using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Data
{
    public class ManufacturerRepository : Repository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(IMongoClient client)
            : base(client.GetDatabase("Warehouse_products").GetCollection<Manufacturer>("manufacturers"))
        {
        }
    }
}