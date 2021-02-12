using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MongoTutorial.Core.Common;

namespace MongoTutorial.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductDto>>> GetProductsAsync()
        {
            var productsFromDb = await _productRepository.GetProductsAsync();

            var products = _mapper.Map<List<ProductDto>>(productsFromDb);
            return Result<List<ProductDto>>.Success(products);
        }

        public async Task<Result<ProductDto>> GetProductByIdAsync(string id)
        {
            var productFromDb = await _productRepository.GetProductByIdAsync(id);
            if (productFromDb == null)
            {
                return Result<ProductDto>.Failure("productId", "Product doesn't exists");
            }

            var product = _mapper.Map<ProductDto>(productFromDb);

            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<ProductDto>> CreateProductAsync(ProductDto product)
        {
            var productToDb = _mapper.Map<Product>(product);
            await _productRepository.CreateProductAsync(productToDb);
            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<ProductDto>> UpdateProductAsync(ProductDto product)
        {
            var productFromDb = await _productRepository.GetProductByIdAsync(product.Id);
            if (productFromDb == null)
            {
                return Result<ProductDto>.Failure("productId", "Product doesn't exists");
            }

            var productToDb = _mapper.Map<Product>(product);
            await _productRepository.UpdateProductAsync(productToDb);
            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<object>> DeleteProductAsync(string id)
        {
            var productFromDb = await _productRepository.GetProductByIdAsync(id);
            if (productFromDb == null)
            {
                return Result<object>.Failure("productId", "Product doesn't exists");
            }

            await _productRepository.DeleteProductAsync(id);
            return Result<object>.Success();
        }
    }
}