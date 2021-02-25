using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoTutorial.Business.Extensions;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Manufacturer;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Core.Settings.CacheSettings;
using MongoTutorial.Domain;

namespace MongoTutorial.Business.Services
{
    public class ManufacturerService : ServiceBase<Manufacturer>, IManufacturerService
    {
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IProductRepository _productRepository;

        public ManufacturerService(IOptions<CacheManufacturerSettings> manufacturerSettings,
            IManufacturerRepository manufacturerRepository, IProductRepository productRepository,
            IDistributedCache distributedCache, IMapper mapper) : base(distributedCache, mapper)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerRepository = manufacturerRepository;
            _productRepository = productRepository;
        }

        public async Task<Result<List<ManufacturerDto>>> GetAllAsync()
        {
            var manufacturersInDb = await _manufacturerRepository.GetAllAsync();
            var manufacturers = Mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds)
        {
            var manufacturersInDb =
                await _manufacturerRepository.GetRangeAsync(manufacturerIds);
            var manufacturers = Mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<ManufacturerDto>> GetAsync(string id)
        {
            var cacheKey = $"Manufacturer-{id}";
            var cache = await DistributedCache.GetStringAsync(cacheKey);
            ManufacturerDto manufacturer;

            if (cache.TryGetValue<Manufacturer>(out var cachedManufacturer))
            {
                manufacturer = Mapper.Map<ManufacturerDto>(cachedManufacturer);

                return Result<ManufacturerDto>.Success(manufacturer);
            }

            var manufacturerInDb = await _manufacturerRepository.GetAsync(id);
            CheckForNull(manufacturerInDb);

            await DistributedCache.SetCacheAsync(cacheKey, manufacturerInDb, _manufacturerSettings);
            manufacturer = Mapper.Map<ManufacturerDto>(manufacturerInDb);

            return Result<ManufacturerDto>.Success(manufacturer);
        }

        public async Task<Result<ManufacturerDto>> CreateAsync(ManufacturerModelDto manufacturer)
        {
            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer);
            var manufacturerToDb = Mapper.Map<Manufacturer>(manufacturerDto);

            var cacheKey = $"Manufacturer-{manufacturerToDb.Id}";
            await DistributedCache.SetCacheAsync(cacheKey, manufacturerToDb, _manufacturerSettings);

            await _manufacturerRepository.CreateAsync(manufacturerToDb);
            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<ManufacturerDto>> UpdateAsync(string manufacturerId, ManufacturerModelDto manufacturer)
        {
            var cacheKey = $"Manufacturer-{manufacturerId}";
            Manufacturer manufacturerInDb;
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                manufacturerInDb = await _manufacturerRepository.GetAsync(manufacturerId);
                CheckForNull(manufacturerInDb);
            }

            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer) with {Id = manufacturerId};
            manufacturerInDb = Mapper.Map<Manufacturer>(manufacturerDto);

            await _manufacturerRepository.UpdateAsync(manufacturerInDb);
            await UpdateManufacturesInProduct(manufacturerInDb.Id, manufacturerInDb);

            await DistributedCache.UpdateAsync(cacheKey, manufacturerInDb);
            await _manufacturerRepository.UpdateAsync(manufacturerInDb);

            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"Manufacturer-{id}";
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                var manufacturerInDb = await _manufacturerRepository.GetAsync(id);
                CheckForNull(manufacturerInDb);
            }

            await UpdateManufacturesInProduct(id);
            await _manufacturerRepository.DeleteAsync(id);

            await DistributedCache.RemoveAsync(cacheKey);

            return Result<object>.Success();
        }

        private async Task UpdateManufacturesInProduct(string id, Manufacturer manufacturer = null)
        {
            var products = await _productRepository.GetRangeByManufacturerId(id);
            foreach (var product in products)
            {
                var manufacturers = product.Manufacturers;
                manufacturers.RemoveAll(m => m.Id == id);

                manufacturers.Add(manufacturer);
                product.Manufacturers = manufacturers;

                await _productRepository.UpdateAsync(product);
            }
        }
    }
}