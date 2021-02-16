using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.Dtos;

namespace MongoTutorial.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<Result<List<ProductDto>>> GetAllAsync();

        Task<Result<ProductDto>> GetAsync(string id);
        
        Task<Result<ProductDto>> CreateAsync(ProductDto product, List<string> manufacturerIds);

        Task<Result<ProductDto>> UpdateAsync(ProductDto product, List<string> manufacturerIds);

        Task<Result<object>> DeleteAsync(string id);
    }
}