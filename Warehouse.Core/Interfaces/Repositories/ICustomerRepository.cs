using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        
        /// <summary>
        /// Gets all customers in database
        /// </summary>
        /// <returns>List of customers</returns>
        Task<List<Customer>> GetAllAsync();
        
        public Task<List<Customer>> GetPageAsync(int page, int pageSize);

        public Task<long> GetCountAsync();

        /// <summary>
        /// Gets customer in database based on provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Customer</returns>
        Task<Customer> GetAsync(string id);

        /// <summary>
        /// Sets customer to database
        /// </summary>
        /// <param name="customer"></param>
        Task CreateAsync(Customer customer);
        
        /// <summary>
        /// Deletes customer from database
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(string id);
    }
}