using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Api.Business;
using Warehouse.Api.Extensions;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Business
{
    public class ProductService : ServiceBase<Product>, IProductService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Products\";
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly CacheProductSettings _productSettings;

        public ProductService(IProductRepository productRepository, IDistributedCache distributedCache, 
            IOptions<CacheProductSettings> productSettings, IOptions<CacheManufacturerSettings> manufacturerSettings, 
            ICustomerRepository customerRepository, IMapper mapper, IManufacturerRepository manufacturerRepository, 
            IFileService fileService) : base(distributedCache, mapper, fileService)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerRepository = manufacturerRepository;
            _customerRepository = customerRepository;
            _productSettings = productSettings.Value;
            _productRepository = productRepository;
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

            productInDb = await FileService.ReadFromFileAsync<Product>(_path, cacheKey);
            CheckForNull(productInDb);

            product = Mapper.Map<ProductDto>(productInDb);

            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<ProductDto>> CreateAsync(ProductModelDto product)
        {
            var productToDb = await GetManufacturersAndCustomer(product, product.ManufacturerIds.ToList());

            var cacheKey = $"Product-{productToDb.Id}";
            await DistributedCache.SetCacheAsync(cacheKey, productToDb, _productSettings);
            await FileService.WriteToFileAsync(productToDb, _path, cacheKey);
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
                              await FileService.ReadFromFileAsync<Product>(_path, cacheKey);

                CheckForNull(productInDb);
            }

            productInDb = await GetManufacturersAndCustomer(product, product.ManufacturerIds.ToList());
            productInDb.Id = productId;
            var result = Mapper.Map<ProductDto>(productInDb) with {Id = productId};

            await DistributedCache.SetCacheAsync(cacheKey, productInDb, _productSettings);
            await FileService.WriteToFileAsync(productInDb, _path, cacheKey);
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
            await FileService.DeleteFileAsync(_path, cacheKey);

            return Result<object>.Success();
        }

        public async Task DeleteManufacturerFromProductAsync(string manufacturerId)
        {
            var products = await _productRepository.GetRangeByManufacturerId(manufacturerId);
            foreach (var product in products)
            {
                product.Manufacturers.RemoveAll(m => m.Id == manufacturerId);
                await _productRepository.UpdateAsync(product);
            }

            await _manufacturerRepository.DeleteAsync(manufacturerId);
        }

        public async Task UpdateManufacturerInProductsAsync(Manufacturer manufacturer)
        {
            var products = await _productRepository.GetRangeByManufacturerId(manufacturer.Id);
            foreach (var product in products)
            {
                await UpdateManufacturerInProductAsync(product, manufacturer.Id, manufacturer);
            }

            await _manufacturerRepository.UpdateAsync(manufacturer);
        }

        public async Task DeleteCustomerFromProductAsync(string customerId)
        {
            var product = await _productRepository.GetByCustomerId(customerId);
            product.Customer = null;
            await _productRepository.UpdateAsync(product);
        }

        public async Task CreateManufacturerAsync(Manufacturer manufacturer)
        {
            await _manufacturerRepository.CreateAsync(manufacturer);
        }

        public async Task CreateCustomerAsync(Customer customer)
        {
            await _customerRepository.CreateAsync(customer);
        }

        private async Task UpdateManufacturerInProductAsync(Product product, string manufacturerId,
            Manufacturer manufacturer)
        {
            var manufacturers = product.Manufacturers.ToList();
            manufacturers.RemoveAll(m => m.Id == manufacturerId);
            manufacturers.Add(manufacturer);
            product.Manufacturers = manufacturers;
            await _productRepository.UpdateAsync(product);
        }

        private async Task<Product> GetManufacturersAndCustomer(ProductModelDto product,
            IReadOnlyCollection<string> manufacturerIds)
        {
            var manufacturersInDb = await GetManufacturers(manufacturerIds);
            var customerInDb = await _customerRepository.GetAsync(product.CustomerId);

            if (manufacturerIds.Count != manufacturersInDb.Count)
            {
                var delta = manufacturerIds.Count - manufacturersInDb.Count;
                throw Result<ProductDto>.Failure("manufacturerIds",
                    $"Provided {delta} non-existed id(s)", HttpStatusCode.BadRequest);
            }

            var productToDb = Mapper.Map<Product>(product);
            var customerToDb = Mapper.Map<Customer>(customerInDb);
            productToDb.Manufacturers = manufacturersInDb;
            productToDb.Customer = customerToDb;

            return productToDb;
        }

        private async Task<List<Manufacturer>> GetManufacturers(IReadOnlyCollection<string> manufacturerIds)
        {
            List<Manufacturer> cachedManufacturers = new();
            var notCachedManufacturerIds = manufacturerIds.ToList();
            foreach (var manufacturerId in manufacturerIds)
            {
                var cacheKey = $"Manufacturer-{manufacturerId}";
                var cache = await DistributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cache))
                {
                    var cachedManufacturer = JsonSerializer.Deserialize<Manufacturer>(cache);
                    cachedManufacturers.Add(cachedManufacturer);
                    notCachedManufacturerIds.Remove(manufacturerId);
                }
            }

            var manufacturersInDb = await _manufacturerRepository.GetRangeAsync(notCachedManufacturerIds);

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