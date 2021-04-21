using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Data
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoClient client)
            : base(client.GetDatabase("Warehouse_products").GetCollection<Customer>("customers"))
        {
        }
    }
}