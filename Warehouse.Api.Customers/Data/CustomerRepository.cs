using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Customers.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customerCollection;

        public CustomerRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse_customers");
            _customerCollection = db.GetCollection<Customer>("customers");
        }
        public Task<List<Customer>> GetAllAsync()
        {
            return _customerCollection.Find(_ => true).ToListAsync();
        }

        public Task<Customer> GetAsync(string id)
        {
            return _customerCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task CreateAsync(Customer customer)
        {
            return _customerCollection.InsertOneAsync(customer);
        }

        public Task UpdateAsync(Customer customer)
        {
            return _customerCollection.ReplaceOneAsync(p => p.Id == customer.Id, customer);
        }

        public Task DeleteAsync(string id)
        {
            return _customerCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }

        public Task<List<Customer>> GetRangeAsync(IEnumerable<string> customerIds)
        {
            return _customerCollection.Find(m => customerIds.Contains(m.Id)).ToListAsync();
        }
    }
}