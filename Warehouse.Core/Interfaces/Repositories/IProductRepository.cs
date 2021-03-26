using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Gets all products in database
        /// </summary>
        /// <returns>List of products</returns>
        Task<List<Product>> GetAllAsync();

        /// <summary>
        /// Gets product in database based on provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Product</returns>
        Task<Product> GetAsync(string id);

        /// <summary>
        /// Sets new product to database
        /// </summary>
        /// <param name="product"></param>
        Task CreateAsync(Product product);

        /// <summary>
        /// Updates product in database
        /// </summary>
        /// <param name="product"></param>
        Task UpdateAsync(Product product);

        /// <summary>
        /// Deletes product from database
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets products in database based on manufacturer id
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <returns>List of products</returns>
        Task<List<Product>> GetRangeByManufacturerId(string manufacturerId);
        
        /// <summary>
        /// Gets products in database based on customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>List of products</returns>
        Task<Product> GetByCustomerId(string customerId);
    }
}