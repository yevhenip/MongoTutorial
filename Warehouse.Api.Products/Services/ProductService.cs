using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly CacheManufacturerSettings _manufacturerSettings;

        public ProductService(ICustomerRepository customerRepository, IMapper mapper, ICacheService cacheService,
            IManufacturerRepository manufacturerRepository, IOptions<CacheManufacturerSettings> manufacturerSettings)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _cacheService = cacheService;
            _manufacturerRepository = manufacturerRepository;
            _manufacturerSettings = manufacturerSettings.Value;
        }

        public async Task<Product> GetManufacturersAndCustomer(ProductModelDto product,
            IReadOnlyCollection<string> manufacturerIds)
        {
            var manufacturersInDb = await GetManufacturers(manufacturerIds);
            var customerInDb =
                await _customerRepository.GetAsync(c => c.Id == product.CustomerId);

            if (customerInDb == null && product.CustomerId != string.Empty && product.CustomerId != "none")
            {
                throw Result<ProductDto>.Failure("customerId",
                    $"Unexisting customer id", HttpStatusCode.BadRequest);
            }

            if (manufacturerIds.Count != manufacturersInDb.Count)
            {
                var delta = manufacturerIds.Count - manufacturersInDb.Count;
                throw Result<ProductDto>.Failure("manufacturerIds",
                    $"Provided {delta} non-existed id(s)", HttpStatusCode.BadRequest);
            }

            var productToDb = _mapper.Map<Product>(product);
            var customerToDb = _mapper.Map<Customer>(customerInDb);
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
                var cache = await _cacheService.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cache))
                {
                    var cachedManufacturer = JsonSerializer.Deserialize<Manufacturer>(cache);
                    cachedManufacturers.Add(cachedManufacturer);
                    notCachedManufacturerIds.Remove(manufacturerId);
                }
            }

            var manufacturersInDb =
                await _manufacturerRepository.GetRangeAsync(m => notCachedManufacturerIds.Contains(m.Id));

            foreach (var manufacturerDto in manufacturersInDb)
            {
                await _cacheService.SetCacheAsync($"Manufacturer-{manufacturerDto.Id}", manufacturerDto,
                    _manufacturerSettings);
            }

            manufacturersInDb.AddRange(cachedManufacturers);

            return manufacturersInDb;
        }
    }
}