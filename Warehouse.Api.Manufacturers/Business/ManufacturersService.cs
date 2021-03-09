using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Api.Extensions;
using Warehouse.Core.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Extensions;
using Warehouse.Core.Interfaces.Messaging.Sender;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Manufacturers.Business
{
    public class ManufacturerService : ServiceBase<Manufacturer>, IManufacturerService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Manufacturers\";
        private readonly ISender _sender;
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly IManufacturerRepository _manufacturerRepository;

        public ManufacturerService(IOptions<CacheManufacturerSettings> manufacturerSettings,
            IManufacturerRepository manufacturerRepository, IDistributedCache distributedCache, IMapper mapper,
            ISender sender, IFileService fileService) : base(distributedCache, mapper, fileService)
        {
            _sender = sender;
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerRepository = manufacturerRepository;
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

            manufacturerInDb = await FileService.ReadFromFileAsync<Manufacturer>(_path, cacheKey);
            CheckForNull(manufacturerInDb);

            manufacturer = Mapper.Map<ManufacturerDto>(manufacturerInDb);

            return Result<ManufacturerDto>.Success(manufacturer);
        }

        public async Task<Result<ManufacturerDto>> CreateAsync(ManufacturerModelDto manufacturer)
        {
            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer);
            var manufacturerToDb = Mapper.Map<Manufacturer>(manufacturerDto);

            var cacheKey = $"Manufacturer-{manufacturerToDb.Id}";
            await DistributedCache.SetCacheAsync(cacheKey, manufacturerToDb, _manufacturerSettings);
            await FileService.WriteToFileAsync(manufacturerToDb, _path, cacheKey);

            await _manufacturerRepository.CreateAsync(manufacturerToDb);
            await _sender.SendMessage(manufacturerToDb, Queues.CreateManufacturerQueue);
            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<ManufacturerDto>> UpdateAsync(string manufacturerId, ManufacturerModelDto manufacturer)
        {
            var cacheKey = $"Manufacturer-{manufacturerId}";
            Manufacturer manufacturerInDb;
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                manufacturerInDb = await _manufacturerRepository.GetAsync(manufacturerId) ??
                                   await FileService.ReadFromFileAsync<Manufacturer>(_path, cacheKey);
                CheckForNull(manufacturerInDb);
            }

            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer) with {Id = manufacturerId};
            manufacturerInDb = Mapper.Map<Manufacturer>(manufacturerDto);

            await _manufacturerRepository.UpdateAsync(manufacturerInDb);
            await _sender.SendMessage(manufacturerDto, Queues.UpdateManufacturerQueue);
            await DistributedCache.UpdateAsync(cacheKey, manufacturerInDb);
            await _manufacturerRepository.UpdateAsync(manufacturerInDb);
            await FileService.WriteToFileAsync(manufacturerInDb, _path, cacheKey);

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

            await _sender.SendMessage(id, Queues.DeleteManufacturerQueue);
            await _manufacturerRepository.DeleteAsync(id);
            await FileService.DeleteFileAsync(_path, cacheKey);

            await DistributedCache.RemoveAsync(cacheKey);

            return Result<object>.Success();
        }
    }
}