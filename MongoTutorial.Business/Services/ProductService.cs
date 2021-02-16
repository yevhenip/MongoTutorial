using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MongoTutorial.Core.Common;

namespace MongoTutorial.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IManufacturerService manufacturerService, IProductRepository productRepository,
            IMapper mapper)
        {
            _manufacturerService = manufacturerService;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductDto>>> GetAllAsync()
        {
            var productsFromDb = await _productRepository.GetAllAsync();

            var products = _mapper.Map<List<ProductDto>>(productsFromDb);
            return Result<List<ProductDto>>.Success(products);
        }

        public async Task<Result<ProductDto>> GetAsync(string id)
        {
            var productFromDb = await _productRepository.GetAsync(id);
            if (productFromDb == null)
            {
                throw Result<ProductDto>.Failure("productId", "Product doesn't exists", HttpStatusCode.BadRequest);
            }

            var product = _mapper.Map<ProductDto>(productFromDb);

            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<ProductDto>> CreateAsync(ProductDto product, List<string> manufacturerIds)
        {
            var productToDb = await GetManufacturers(product, manufacturerIds);
            var result = _mapper.Map<ProductDto>(productToDb);

            await _productRepository.CreateProductAsync(productToDb);
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<ProductDto>> UpdateAsync(ProductDto product, List<string> manufacturerIds)
        {
            var productFromDb = await _productRepository.GetAsync(product.Id);
            if (productFromDb == null)
            {
                throw Result<ProductDto>.Failure("productId", "Product doesn't exists", HttpStatusCode.BadRequest);
            }

            var productToDb = await GetManufacturers(product, manufacturerIds);
            var result = _mapper.Map<ProductDto>(productToDb);

            //TODO: Update user in product document
            await _productRepository.UpdateAsync(productToDb);
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var productFromDb = await _productRepository.GetAsync(id);
            if (productFromDb == null)
            {
                return Result<object>.Failure("productId", "Product doesn't exists");
            }

            await _productRepository.DeleteAsync(id);
            return Result<object>.Success();
        }

        private async Task<Product> GetManufacturers(ProductDto product, IReadOnlyCollection<string> manufacturerIds)
        {
            var manufacturersFromDb =
                (await _manufacturerService.GetRangeAsync(manufacturerIds)).Data;

            if (manufacturerIds.Count != manufacturersFromDb.Count)
            {
                var delta = manufacturerIds.Count - manufacturersFromDb.Count;
                throw Result<ProductDto>.Failure("manufacturerIds", $"Provided {delta} non-existed id(s)",
                    HttpStatusCode.BadRequest);
            }

            var manufacturers = _mapper.Map<List<Manufacturer>>(manufacturersFromDb);
            var productToDb = _mapper.Map<Product>(product);
            productToDb.Manufacturers = manufacturers;

            return productToDb;
        }
    }
}