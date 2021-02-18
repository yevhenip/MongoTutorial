using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Product;

namespace MongoTutorial.Business.Services
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductRepository _productRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ProductService(IManufacturerService manufacturerService, IProductRepository productRepository,
            IUserService userService, IMapper mapper)
        {
            _manufacturerService = manufacturerService;
            _productRepository = productRepository;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductDto>>> GetAllAsync()
        {
            var productsInDb = await _productRepository.GetAllAsync();
            var products = _mapper.Map<List<ProductDto>>(productsInDb);

            return Result<List<ProductDto>>.Success(products);
        }

        public async Task<Result<ProductDto>> GetAsync(string id)
        {
            var productInDb = await _productRepository.GetAsync(id);
            CheckForNull(productInDb);

            var product = _mapper.Map<ProductDto>(productInDb);

            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<ProductDto>> CreateAsync(ProductModelDto product)
        {
            var productToDb = await GetManufacturersAndUser(product, product.ManufacturerIds.ToList());
            var result = _mapper.Map<ProductDto>(productToDb);

            await _productRepository.CreateAsync(productToDb);
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<ProductDto>> UpdateAsync(string productId, ProductModelDto product)
        {
            var productInDb = await _productRepository.GetAsync(productId);
            CheckForNull(productInDb);

            productInDb = await GetManufacturersAndUser(product, product.ManufacturerIds.ToList());
            var result = _mapper.Map<ProductDto>(productInDb) with {Id = productId};

            await _productRepository.UpdateAsync(productInDb);
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var productInDb = await _productRepository.GetAsync(id);
            CheckForNull(productInDb);

            await _productRepository.DeleteAsync(id);
            return Result<object>.Success();
        }

        private async Task<Product> GetManufacturersAndUser(ProductModelDto product,
            IReadOnlyCollection<string> manufacturerIds)
        {
            var manufacturersInDb =
                (await _manufacturerService.GetRangeAsync(manufacturerIds)).Data;
            var userInDb = (await _userService.GetAsync(product.UserId)).Data;

            if (manufacturerIds.Count != manufacturersInDb.Count)
            {
                var delta = manufacturerIds.Count - manufacturersInDb.Count;
                throw Result<ProductDto>.Failure("manufacturerIds",
                    $"Provided {delta} non-existed id(s)", HttpStatusCode.BadRequest);
            }

            var productToDb = _mapper.Map<Product>(product);
            var manufacturers = _mapper.Map<List<Manufacturer>>(manufacturersInDb);
            var user = _mapper.Map<User>(userInDb);
            productToDb.Manufacturers = manufacturers;
            productToDb.User = user;

            return productToDb;
        }
    }
}