using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoTutorial.Business.Extensions;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Manufacturer;
using MongoTutorial.Core.DTO.Product;
using MongoTutorial.Core.Settings.CacheSettings;

namespace MongoTutorial.Business.Services
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\wwwroot\Products\";
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductRepository _productRepository;
        private readonly CacheProductSettings _productSettings;
        private readonly IUserRepository _userRepository;

        public ProductService(IManufacturerService manufacturerService, IProductRepository productRepository,
            IDistributedCache distributedCache, IOptions<CacheProductSettings> productSettings,
            IOptions<CacheManufacturerSettings> manufacturerSettings, IUserRepository userRepository, IMapper mapper)
            : base(distributedCache, mapper)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerService = manufacturerService;
            _productSettings = productSettings.Value;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<List<ProductDto>>> GetAllAsync()
        {
            var productsInDb = await _productRepository.GetAllAsync();
            var products = Mapper.Map<List<ProductDto>>(productsInDb);

            return Result<List<ProductDto>>.Success(products);
        }

        public async Task<Result<ProductDto>> GetAsync(string id)
        {
            var cacheKey = $"Product-{id}";
            var cache = await DistributedCache.GetStringAsync(cacheKey);
            ProductDto product;

            if (cache.TryGetValue<Product>(out var cachedProduct))
            {
                product = Mapper.Map<ProductDto>(cachedProduct);

                return Result<ProductDto>.Success(product);
            }

            var productInDb = await _productRepository.GetAsync(id);

            if (productInDb is not null)
            {
                product = Mapper.Map<ProductDto>(productInDb);
                await DistributedCache.SetCacheAsync(cacheKey, productInDb, _productSettings);

                return Result<ProductDto>.Success(product);
            }

            productInDb = await FileExtensions.ReadFromFileAsync<Product>(_path, cacheKey + ".json");
            CheckForNull(productInDb);

            product = Mapper.Map<ProductDto>(productInDb);
            await DistributedCache.SetCacheAsync(cacheKey, productInDb, _productSettings);

            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<ProductDto>> CreateAsync(ProductModelDto product)
        {
            var productToDb = await GetManufacturersAndUser(product, product.ManufacturerIds.ToList());

            var cacheKey = $"Product-{productToDb.Id}";
            await DistributedCache.SetCacheAsync(cacheKey, productToDb, _productSettings);
            await productToDb.WriteToFileAsync(_path, cacheKey + ".json");
            var result = Mapper.Map<ProductDto>(productToDb);

            await _productRepository.CreateAsync(productToDb);
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<ProductDto>> UpdateAsync(string productId, ProductModelDto product)
        {
            var cacheKey = $"Product-{productId}";
            Product productInDb;
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                productInDb = await _productRepository.GetAsync(productId) ??
                              await FileExtensions.ReadFromFileAsync<Product>(_path, cacheKey + ".json");

                CheckForNull(productInDb);
            }

            productInDb = await GetManufacturersAndUser(product, product.ManufacturerIds.ToList());
            productInDb.Id = productId;
            var result = Mapper.Map<ProductDto>(productInDb) with {Id = productId};

            await DistributedCache.SetCacheAsync(cacheKey, productInDb, _productSettings);
            await productInDb.WriteToFileAsync(_path, cacheKey + ".json");
            await _productRepository.UpdateAsync(productInDb);
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"Product-{id}";
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                var productInDb = await _productRepository.GetAsync(id);
                CheckForNull(productInDb);
            }

            await _productRepository.DeleteAsync(id);
            await DistributedCache.RemoveAsync(cacheKey);

            return Result<object>.Success();
        }

        private async Task<Product> GetManufacturersAndUser(ProductModelDto product,
            IReadOnlyCollection<string> manufacturerIds)
        {
            var manufacturersInDb = await GetManufacturers(manufacturerIds);
            var userInDb = await _userRepository.GetAsync(product.UserId);

            if (manufacturerIds.Count != manufacturersInDb.Count)
            {
                var delta = manufacturerIds.Count - manufacturersInDb.Count;
                throw Result<ProductDto>.Failure("manufacturerIds",
                    $"Provided {delta} non-existed id(s)", HttpStatusCode.BadRequest);
            }

            var productToDb = Mapper.Map<Product>(product);
            var manufacturers = Mapper.Map<List<Manufacturer>>(manufacturersInDb);
            var user = Mapper.Map<User>(userInDb);
            productToDb.Manufacturers = manufacturers;
            productToDb.User = user;

            return productToDb;
        }

        private async Task<List<ManufacturerDto>> GetManufacturers(IReadOnlyCollection<string> manufacturerIds)
        {
            List<ManufacturerDto> cachedManufacturers = new();
            var notCachedManufacturerIds = manufacturerIds.ToList();
            foreach (var manufacturerId in manufacturerIds)
            {
                var cacheKey = $"Manufacturer-{manufacturerId}";
                var cache = await DistributedCache.GetStringAsync(cacheKey);
                if (cache is not null)
                {
                    var cachedManufacturer = JsonSerializer.Deserialize<Manufacturer>(cache);
                    cachedManufacturers.Add(Mapper.Map<ManufacturerDto>(cachedManufacturer));
                    notCachedManufacturerIds.Remove(manufacturerId);
                }
            }

            var manufacturersInDb =
                (await _manufacturerService.GetRangeAsync(notCachedManufacturerIds)).Data;

            foreach (var manufacturerDto in manufacturersInDb)
            {
                await DistributedCache.SetCacheAsync($"Manufacturer-{manufacturerDto.Id}", manufacturerDto,
                    _manufacturerSettings);
            }

            manufacturersInDb.AddRange(cachedManufacturers);

            return manufacturersInDb;
        }
    }
}