using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.Dtos;

namespace MongoTutorial.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<Result<List<ProductDto>>> GetProductsAsync();

        Task<Result<ProductDto>> GetProductByIdAsync(string id);

        Task<Result<ProductDto>> CreateProductAsync(ProductDto product);

        Task<Result<ProductDto>> UpdateProductAsync(ProductDto product);

        Task<Result<object>> DeleteProductAsync(string id);
    }
}