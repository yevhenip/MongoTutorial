using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Product;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<Result<List<ProductDto>>> GetAllAsync();

        Task<Result<ProductDto>> GetAsync(string id);

        Task<Result<ProductDto>> CreateAsync(ProductModelDto product);

        Task<Result<ProductDto>> UpdateAsync(string productId, ProductModelDto product);

        Task<Result<object>> DeleteAsync(string id);
        Task DeleteManufacturerFromProductAsync(string manufacturerId);
        Task UpdateManufacturerInProductsAsync(Manufacturer manufacturer);
        Task DeleteCustomerFromProductAsync(string customerId);
        Task CreateManufacturerAsync(Manufacturer manufacturer);
        Task CreateCustomerAsync(Customer customer);
    }
}