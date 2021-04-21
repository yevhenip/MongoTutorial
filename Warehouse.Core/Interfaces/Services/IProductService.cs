using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.DTO.Product;

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

        public Task<Result<PageDataDto<ProductDto>>> GetPageAsync(int page, int pageSize);

        Task<Result<byte[]>> GetExportFileAsync();

        /// <summary>
        /// Creates, sets to cache, file system
        /// </summary>
        /// <param name="product"></param>
        /// <param name="userName"></param>
        /// <returns>Created product</returns>
        Task<Result<ProductDto>> CreateAsync(ProductModelDto product, string userName);

        /// <summary>
        /// Updates product in database, cache, filesystem
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="product"></param>
        /// <param name="userName"></param>
        /// <returns>Updated product</returns>
        Task<Result<ProductDto>> UpdateAsync(string productId, ProductModelDto product, string userName);

        /// <summary>
        /// Deletes product from cache, file system, database
        /// </summary>
        /// <param name="id"></param>
        Task<Result<object>> DeleteAsync(string id);

        /// <summary>
        /// Deletes manufacturer from product
        /// </summary>
        /// <param name="manufacturer"></param>
        Task DeleteManufacturerFromProductAsync(DeletedManufacturer manufacturer);

        /// <summary>
        /// Updates manufacturer in product
        /// </summary>
        /// <param name="manufacturer"></param>
        Task UpdateManufacturerInProductsAsync(UpdatedManufacturer manufacturer);

        /// <summary>
        /// Deletes customer from product
        /// </summary>
        /// <param name="customer"></param>
        Task DeleteCustomerFromProductAsync(DeletedCustomer customer);

        /// <summary>
        /// Creates received manufacturer
        /// </summary>
        /// <param name="manufacturer"></param>
        Task CreateManufacturerAsync(CreatedManufacturer manufacturer);

        /// <summary>
        /// Creates received customer
        /// </summary>
        /// <param name="customer"></param>
        Task CreateCustomerAsync(CreatedCustomer customer);
    }
}