using System.Collections.Generic;
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
            var db = client.GetDatabase("Products");
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

        public Task DeleteAsync(string id)
        {
            return _customerCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }
    }
}