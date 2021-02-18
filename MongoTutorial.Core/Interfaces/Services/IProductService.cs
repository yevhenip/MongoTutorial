using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Product;

namespace MongoTutorial.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<Result<List<ProductDto>>> GetAllAsync();

        Task<Result<ProductDto>> GetAsync(string id);

        Task<Result<ProductDto>> CreateAsync(ProductModelDto product);

        Task<Result<ProductDto>> UpdateAsync(string productId, ProductModelDto product);

        Task<Result<object>> DeleteAsync(string id);
    }
}