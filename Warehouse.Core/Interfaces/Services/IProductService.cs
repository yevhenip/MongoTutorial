using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.DTO.Product;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<Product> GetManufacturersAndCustomer(ProductModelDto product, IReadOnlyCollection<string> manufacturerIds);
    }
}