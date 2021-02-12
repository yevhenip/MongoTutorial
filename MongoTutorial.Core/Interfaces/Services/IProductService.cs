using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Core.Dtos;

namespace MongoTutorial.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetProductsAsync();

        Task<ProductDto> GetProductByIdAsync(string id);

        Task<ProductDto> CreateProductAsync(ProductDto product);

        Task UpdateProductAsync(ProductDto product);

        Task DeleteProductAsync(string id);
    }
}