using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Settings;
using Warehouse.Domain;

namespace Warehouse.Api.Customers.Data
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IMongoClient client, IOptions<PollySettings> pollySettings)
            : base(client.GetDatabase("Warehouse_customers").GetCollection<Customer>("customers"), pollySettings.Value)
        {
        }
    }
}