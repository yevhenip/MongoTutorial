using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Product;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>List of products</returns>
        Task<Result<List<ProductDto>>> GetAllAsync();

        /// <summary>
        /// Tries to get product firstly from cache, then from database, then from file. If product still null throws an exception
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Product</returns>
        Task<Result<ProductDto>> GetAsync(string id);

        /// <summary>
        /// Creates, sets to cache, file system
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Created product</returns>
        Task<Result<ProductDto>> CreateAsync(ProductModelDto product);

        /// <summary>
        /// Updates product in database, cache, filesystem
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="product"></param>
        /// <returns>Updated product</returns>
        Task<Result<ProductDto>> UpdateAsync(string productId, ProductModelDto product);

        /// <summary>
        /// Deletes product from cache, file system, database
        /// </summary>
        /// <param name="id"></param>
        Task<Result<object>> DeleteAsync(string id);
        
        /// <summary>
        /// Deletes manufacturer from product
        /// </summary>
        /// <param name="manufacturerId"></param>
        Task DeleteManufacturerFromProductAsync(string manufacturerId);
        
        /// <summary>
        /// Updates manufacturer in product
        /// </summary>
        /// <param name="manufacturer"></param>
        Task UpdateManufacturerInProductsAsync(Manufacturer manufacturer);
        
        /// <summary>
        /// Deletes customer from product
        /// </summary>
        /// <param name="customerId"></param>
        Task DeleteCustomerFromProductAsync(string customerId);
        
        /// <summary>
        /// Creates received manufacturer
        /// </summary>
        /// <param name="manufacturer"></param>
        Task CreateManufacturerAsync(Manufacturer manufacturer);
        
        /// <summary>
        /// Creates received customer
        /// </summary>
        /// <param name="customer"></param>
        Task CreateCustomerAsync(Customer customer);
    }
}