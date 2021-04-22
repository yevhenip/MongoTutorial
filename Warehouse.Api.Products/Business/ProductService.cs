using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Api.Business;
using Warehouse.Api.Extensions;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        public ProductService(IProductRepository productRepository, IDistributedCache distributedCache, IBus bus, 
            IOptions<CacheProductSettings> productSettings, IOptions<CacheManufacturerSettings> manufacturerSettings, 
            ICustomerRepository customerRepository, IMapper mapper, IManufacturerRepository manufacturerRepository, 
            IFileService fileService, IOptions<PollySettings> pollySettings) : base(mapper, bus, pollySettings.Value, 
            distributedCache, fileService)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerRepository = manufacturerRepository;
            _customerRepository = customerRepository;
            _productSettings = productSettings.Value;
            _productRepository = productRepository;
        }

        public async Task<Result<List<ProductDto>>> GetAllAsync()
        {
            var productsInDb = await DbPolicy.ExecuteAsync(() => _productRepository.GetRangeAsync(_ => true));
            var products = Mapper.Map<List<ProductDto>>(productsInDb);

            return Result<List<ProductDto>>.Success(products);
        }

        public async Task<Result<PageDataDto<ProductDto>>> GetPageAsync(int page, int pageSize)
        {
            var productsInDb = await DbPolicy.ExecuteAsync(() => _productRepository.GetPageAsync(page, pageSize));
            var count = await DbPolicy.ExecuteAsync(() => _productRepository.GetCountAsync(_ => true));
            var products = Mapper.Map<List<ProductDto>>(productsInDb);
            PageDataDto<ProductDto> pageData = new(products, count);

            return Result<PageDataDto<ProductDto>>.Success(pageData);
        }

        public async Task<Result<ProductDto>> GetAsync(string id)
        {
            var cacheKey = $"Product-{id}";
            var cache = await CachingPolicy.ExecuteAsync(() => DistributedCache.GetStringAsync(cacheKey));
            ProductDto product;

            if (cache.TryGetValue<Product>(out var cachedProduct))
            {
                product = Mapper.Map<ProductDto>(cachedProduct);

                return Result<ProductDto>.Success(product);
            }

            var productInDb = await DbPolicy.ExecuteAsync(() => _productRepository.GetAsync(p => p.Id == id));

            if (productInDb is not null)
            {
                product = Mapper.Map<ProductDto>(productInDb);
                await CachingPolicy.ExecuteAsync(() =>
                    DistributedCache.SetCacheAsync(cacheKey, productInDb, _productSettings));

                return Result<ProductDto>.Success(product);
            }

            productInDb = await FileService.ReadFromFileAsync<Product>(_path, cacheKey);
            CheckForNull(productInDb);

            product = Mapper.Map<ProductDto>(productInDb);

            return Result<ProductDto>.Success(product);
        }

        public async Task<Result<byte[]>> GetExportFileAsync()
        {
            var productsInDb = await DbPolicy.ExecuteAsync(() => _productRepository.GetRangeAsync(_ => true));
            var exportProducts = Mapper.Map<List<ExportProduct>>(productsInDb);
            StringBuilder builder = new();
            foreach (var product in exportProducts)
            {
                builder.Append(product);
                builder.Append(Environment.NewLine);
            }

            var bytes = Encoding.UTF8.GetBytes(builder.ToString());
            return Result<byte[]>.Success(bytes);
        }

        public async Task<Result<ProductDto>> CreateAsync(ProductModelDto product, string userName)
        {
            var productToDb = await GetManufacturersAndCustomer(product, product.ManufacturerIds.ToList());

            var cacheKey = $"Product-{productToDb.Id}";
            await CachingPolicy.ExecuteAsync(() =>
                DistributedCache.SetCacheAsync(cacheKey, productToDb, _productSettings));
            await FileService.WriteToFileAsync(productToDb, _path, cacheKey);

            var result = Mapper.Map<ProductDto>(productToDb);
            LogDto log =
                new(Guid.NewGuid().ToString(), userName, "added product", JsonSerializer.Serialize(result,
                    JsonSerializerOptions), DateTime.UtcNow);

            await RabbitPolicy.ExecuteAsync(() => Bus.PubSub.PublishAsync(log));
            await DbPolicy.ExecuteAsync(() => _productRepository.CreateAsync(productToDb));
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<ProductDto>> UpdateAsync(string productId, ProductModelDto product, string userName)
        {
            var cacheKey = $"Product-{productId}";
            Product productInDb;
            if (!await CachingPolicy.ExecuteAsync(() => DistributedCache.IsExistsAsync(cacheKey)))
            {
                productInDb = await DbPolicy.ExecuteAsync(() => _productRepository.GetAsync(p => p.Id == productId)) ??
                              await FileService.ReadFromFileAsync<Product>(_path, cacheKey);

                CheckForNull(productInDb);
            }

            productInDb = await GetManufacturersAndCustomer(product, product.ManufacturerIds.ToList());
            productInDb.Id = productId;
            var result = Mapper.Map<ProductDto>(productInDb) with {Id = productId};
            LogDto log =
                new(Guid.NewGuid().ToString(), userName, "edited product", JsonSerializer.Serialize(result,
                    JsonSerializerOptions), DateTime.UtcNow);

            await CachingPolicy.ExecuteAsync(() =>
                DistributedCache.SetCacheAsync(cacheKey, productInDb, _productSettings));
            await FileService.WriteToFileAsync(productInDb, _path, cacheKey);
            await RabbitPolicy.ExecuteAsync(() => Bus.PubSub.PublishAsync(log));
            await DbPolicy.ExecuteAsync(() => _productRepository.UpdateAsync(p => p.Id == productInDb.Id, productInDb));
            return Result<ProductDto>.Success(result);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"Product-{id}";
            if (!await CachingPolicy.ExecuteAsync(() => DistributedCache.IsExistsAsync(cacheKey)))
            {
                var productInDb = await DbPolicy.ExecuteAsync(() => _productRepository.GetAsync(p => p.Id == id));
                CheckForNull(productInDb);
            }

            await DbPolicy.ExecuteAsync(() => _productRepository.DeleteAsync(p => p.Id == id));
            await CachingPolicy.ExecuteAsync(() => DistributedCache.RemoveAsync(cacheKey));
            await FileService.DeleteFileAsync(_path, cacheKey);

            return Result<object>.Success();
        }

        public async Task DeleteManufacturerFromProductAsync(DeletedManufacturer manufacturer)
        {
            var products = await DbPolicy.ExecuteAsync(() => _productRepository.GetRangeAsync(p =>
                p.Manufacturers.Any(m => m.Id == manufacturer.Id)));
            foreach (var product in products)
            {
                product.Manufacturers.RemoveAll(m => m.Id == manufacturer.Id);
                await DbPolicy.ExecuteAsync(() => _productRepository.UpdateAsync(p => p.Id == product.Id, product));
            }

            await DbPolicy.ExecuteAsync(() => _manufacturerRepository.DeleteAsync(m => m.Id == manufacturer.Id));
        }

        public async Task UpdateManufacturerInProductsAsync(UpdatedManufacturer manufacturer)
        {
            var products = await DbPolicy.ExecuteAsync(() => _productRepository.GetRangeAsync(p =>
                p.Manufacturers.Any(m => m.Id == manufacturer.Manufacturer.Id)));
            foreach (var product in products)
            {
                await UpdateManufacturerInProductAsync(product, manufacturer.Manufacturer.Id,
                    manufacturer.Manufacturer);
            }

            await DbPolicy.ExecuteAsync(() => _manufacturerRepository.UpdateAsync(
                m => m.Id == manufacturer.Manufacturer.Id,
                manufacturer.Manufacturer));
        }

        public async Task DeleteCustomerFromProductAsync(DeletedCustomer customer)
        {
            var products =
                await DbPolicy.ExecuteAsync(() => _productRepository.GetRangeAsync(p => p.Customer.Id == customer.Id));
            foreach (var product in products)
            {
                product.Customer = null;
                await DbPolicy.ExecuteAsync(() => _productRepository.UpdateAsync(p => p.Id == product.Id, product));
            }

            await DbPolicy.ExecuteAsync(() => _customerRepository.DeleteAsync(c => c.Id == customer.Id));
        }

        public async Task CreateManufacturerAsync(CreatedManufacturer manufacturer)
        {
            await DbPolicy.ExecuteAsync(() => _manufacturerRepository.CreateAsync(manufacturer.Manufacturer));
        }

        public async Task CreateCustomerAsync(CreatedCustomer customer)
        {
            await DbPolicy.ExecuteAsync(() => _customerRepository.CreateAsync(customer.Customer));
        }

        private async Task UpdateManufacturerInProductAsync(Product product, string manufacturerId,
            Manufacturer manufacturer)
        {
            var manufacturers = product.Manufacturers.ToList();
            manufacturers.RemoveAll(m => m.Id == manufacturerId);
            manufacturers.Add(manufacturer);
            product.Manufacturers = manufacturers;
            await DbPolicy.ExecuteAsync(() => _productRepository.UpdateAsync(p => p.Id == product.Id, product));
        }

        private async Task<Product> GetManufacturersAndCustomer(ProductModelDto product,
            IReadOnlyCollection<string> manufacturerIds)
        {
            var manufacturersInDb = await GetManufacturers(manufacturerIds);
            var customerInDb =
                await DbPolicy.ExecuteAsync(() => _customerRepository.GetAsync(c => c.Id == product.CustomerId));

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
                var cache = await CachingPolicy.ExecuteAsync(() => DistributedCache.GetStringAsync(cacheKey));
                if (!string.IsNullOrEmpty(cache))
                {
                    var cachedManufacturer = JsonSerializer.Deserialize<Manufacturer>(cache);
                    cachedManufacturers.Add(cachedManufacturer);
                    notCachedManufacturerIds.Remove(manufacturerId);
                }
            }

            var manufacturersInDb = await DbPolicy.ExecuteAsync(() =>
                _manufacturerRepository.GetRangeAsync(m => notCachedManufacturerIds.Contains(m.Id)));

            foreach (var manufacturerDto in manufacturersInDb)
            {
                await CachingPolicy.ExecuteAsync(() => DistributedCache.SetCacheAsync(
                    $"Manufacturer-{manufacturerDto.Id}", manufacturerDto, _manufacturerSettings));
            }

            manufacturersInDb.AddRange(cachedManufacturers);

            return manufacturersInDb;
        }
    }
}