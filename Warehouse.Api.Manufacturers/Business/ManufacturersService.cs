using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Core.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Extensions;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Manufacturers.Business
{
    public class ManufacturerService : ServiceBase<Manufacturer>, IManufacturerService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\wwwroot\Manufacturers\";
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
            if (manufacturerInDb is not null)
            {
                await DistributedCache.SetCacheAsync(cacheKey, manufacturerInDb, _manufacturerSettings);
                manufacturer = Mapper.Map<ManufacturerDto>(manufacturerInDb);

                return Result<ManufacturerDto>.Success(manufacturer);
            }

            manufacturerInDb = await FileExtensions.ReadFromFileAsync<Manufacturer>(_path, cacheKey + ".json");
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
            await manufacturerToDb.WriteToFileAsync(_path, cacheKey + ".json");

            await _manufacturerRepository.CreateAsync(manufacturerToDb);
            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<ManufacturerDto>> UpdateAsync(string manufacturerId, ManufacturerModelDto manufacturer)
        {
            var cacheKey = $"Manufacturer-{manufacturerId}";
            Manufacturer manufacturerInDb;
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                manufacturerInDb = await _manufacturerRepository.GetAsync(manufacturerId) ??
                                   await FileExtensions.ReadFromFileAsync<Manufacturer>(_path, cacheKey + ".json");
                CheckForNull(manufacturerInDb);
            }

            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer) with {Id = manufacturerId};
            manufacturerInDb = Mapper.Map<Manufacturer>(manufacturerDto);

            await _manufacturerRepository.UpdateAsync(manufacturerInDb);
            await UpdateManufacturesInProduct(manufacturerInDb.Id, manufacturerInDb);

            await DistributedCache.UpdateAsync(cacheKey, manufacturerInDb);
            await _manufacturerRepository.UpdateAsync(manufacturerInDb);
            await manufacturerInDb.WriteToFileAsync(_path, cacheKey + ".json");

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