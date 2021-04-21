using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Api.Business;
using Warehouse.Api.Extensions;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Warehouse.Api.Manufacturers.Business
{
    public class ManufacturerService : ServiceBase<Manufacturer>, IManufacturerService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Manufacturers\";
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly IManufacturerRepository _manufacturerRepository;

        public ManufacturerService(IOptions<CacheManufacturerSettings> manufacturerSettings,
            IManufacturerRepository manufacturerRepository, IDistributedCache distributedCache, IMapper mapper,
            IBus bus, IFileService fileService) : base(mapper, distributedCache, bus, fileService)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Result<List<ManufacturerDto>>> GetAllAsync()
        {
            var manufacturersInDb = await _manufacturerRepository.GetRangeAsync(m => true);
            var manufacturers = Mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<PageDataDto<ManufacturerDto>>> GetPageAsync(int page, int pageSize)
        {
            var manufacturersInDb = await _manufacturerRepository.GetPageAsync(page, pageSize);
            var count = await _manufacturerRepository.GetCountAsync(_ => true);
            var manufacturers = Mapper.Map<List<ManufacturerDto>>(manufacturersInDb);
            PageDataDto<ManufacturerDto> pageData = new(manufacturers, count);

            return Result<PageDataDto<ManufacturerDto>>.Success(pageData);
        }

        public async Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds)
        {
            var manufacturersInDb =
                await _manufacturerRepository.GetRangeAsync(m => manufacturerIds.Contains(m.Id));
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

            var manufacturerInDb = await _manufacturerRepository.GetAsync(m => m.Id == id);
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

        public async Task<Result<ManufacturerDto>> CreateAsync(ManufacturerModelDto manufacturer, string userName)
        {
            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer);
            var manufacturerToDb = Mapper.Map<Manufacturer>(manufacturerDto);

            var cacheKey = $"Manufacturer-{manufacturerToDb.Id}";
            LogDto log =
                new(Guid.NewGuid().ToString(), userName, "added manufacturer", JsonSerializer.Serialize(manufacturerDto,
                    JsonSerializerOptions), DateTime.UtcNow);

            await DistributedCache.SetCacheAsync(cacheKey, manufacturerToDb, _manufacturerSettings);
            await FileService.WriteToFileAsync(manufacturerToDb, _path, cacheKey);

            await _manufacturerRepository.CreateAsync(manufacturerToDb);
            await Bus.PubSub.PublishAsync(new CreatedManufacturer(manufacturerToDb));
            await Bus.PubSub.PublishAsync(log);
            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<ManufacturerDto>> UpdateAsync(string manufacturerId, ManufacturerModelDto manufacturer,
            string userName)
        {
            var cacheKey = $"Manufacturer-{manufacturerId}";
            Manufacturer manufacturerInDb;
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                manufacturerInDb = await _manufacturerRepository.GetAsync(m => m.Id == manufacturerId) ??
                                   await FileService.ReadFromFileAsync<Manufacturer>(_path, cacheKey);
                CheckForNull(manufacturerInDb);
            }

            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer) with {Id = manufacturerId};
            manufacturerInDb = Mapper.Map<Manufacturer>(manufacturerDto);
            LogDto log =
                new(Guid.NewGuid().ToString(), userName, "edited manufacturer", JsonSerializer.Serialize(
                    manufacturerDto,
                    JsonSerializerOptions), DateTime.UtcNow);

            await _manufacturerRepository.UpdateAsync(m => m.Id == manufacturerInDb.Id, manufacturerInDb);
            await Bus.PubSub.PublishAsync(new UpdatedManufacturer(manufacturerInDb));
            await Bus.PubSub.PublishAsync(log);
            await DistributedCache.UpdateAsync(cacheKey, manufacturerInDb);
            await FileService.WriteToFileAsync(manufacturerInDb, _path, cacheKey);

            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"Manufacturer-{id}";
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                var manufacturerInDb = await _manufacturerRepository.GetAsync(m => m.Id == id);
                CheckForNull(manufacturerInDb);
            }

            await Bus.PubSub.PublishAsync(new DeletedManufacturer(id));
            await _manufacturerRepository.DeleteAsync(m => m.Id == id);
            await FileService.DeleteFileAsync(_path, cacheKey);

            await DistributedCache.RemoveAsync(cacheKey);

            return Result<object>.Success();
        }
    }
}