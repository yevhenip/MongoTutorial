using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        Task<Product> GetAsync(string id);

        Task CreateAsync(Product product);

        Task UpdateAsync(Product product);

        Task DeleteAsync(string id);

        Task<List<Product>> GetRangeByManufacturerId(string manufacturerId);
        Task<Product> GetByCustomerId(string customerId);
    }
}