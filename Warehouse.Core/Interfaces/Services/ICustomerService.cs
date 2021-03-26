using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        /// <summary>
        /// Gets all customers
        /// </summary>
        /// <returns>List of customers</returns>
        Task<Result<List<CustomerDto>>> GetAllAsync();

        /// <summary>
        /// Tries to get customer firstly from cache, then from database, then from file. If customer still null throws an exception
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Customer</returns>
        Task<Result<CustomerDto>> GetAsync(string id);

        /// <summary>
        /// Deletes customer from cache, file system, database and sends a message with customer id
        /// </summary>
        /// <param name="id"></param>
        Task<Result<object>> DeleteAsync(string id);
        
        /// <summary>
        /// Creates customer, adds to cache, file system and sends message with customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>Created customer</returns>
        Task<Result<CustomerDto>> CreateAsync(CustomerDto customer);
    }
}