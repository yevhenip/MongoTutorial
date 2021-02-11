using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Domain;

namespace MongoTutorial.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync();

        Task<Product> GetProductByIdAsync(string id);
        Task CreateProductAsync(Product product);

        Task UpdateProductAsync(Product product);

        Task DeleteProductAsync(string id);
    }
}