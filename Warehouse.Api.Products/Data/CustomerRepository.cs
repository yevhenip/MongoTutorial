using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _customerCollection;

        public CustomerRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse_products");
            _customerCollection = db.GetCollection<Customer>("customers");
        }

        public Task<List<Customer>> GetAllAsync()
        {
            return _customerCollection.Find(_ => true).ToListAsync();
        }

        public Task<List<Customer>> GetPageAsync(int page, int pageSize)
        {
            return _customerCollection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public Task<long> GetCountAsync()
        {
            return _customerCollection.CountDocumentsAsync(_ => true);
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