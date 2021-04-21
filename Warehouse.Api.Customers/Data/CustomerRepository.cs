using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Customers.Data
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoClient client)
            : base(client.GetDatabase("Warehouse_customers").GetCollection<Customer>("customers"))
        {
        }
    }
}