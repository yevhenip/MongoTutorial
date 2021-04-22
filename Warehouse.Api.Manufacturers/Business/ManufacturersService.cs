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
using Warehouse.Core.Settings;
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

        public ManufacturerService(IOptions<CacheManufacturerSettings> manufacturerSettings, IBus bus,
            IManufacturerRepository manufacturerRepository, IDistributedCache distributedCache, IMapper mapper,
            IFileService fileService, IOptions<PollySettings> pollySettings) : base(mapper, bus, pollySettings.Value,
            distributedCache, fileService)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerRepository = manufacturerRepository;
        }

        public async Task<Result<List<ManufacturerDto>>> GetAllAsync()
        {
            var manufacturersInDb = await DbPolicy.ExecuteAsync(() => _manufacturerRepository.GetRangeAsync(m => true));
            var manufacturers = Mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<PageDataDto<ManufacturerDto>>> GetPageAsync(int page, int pageSize)
        {
            var manufacturersInDb =
                await DbPolicy.ExecuteAsync(() => _manufacturerRepository.GetPageAsync(page, pageSize));
            var count = await DbPolicy.ExecuteAsync(() => _manufacturerRepository.GetCountAsync(_ => true));
            var manufacturers = Mapper.Map<List<ManufacturerDto>>(manufacturersInDb);
            PageDataDto<ManufacturerDto> pageData = new(manufacturers, count);

            return Result<PageDataDto<ManufacturerDto>>.Success(pageData);
        }

        public async Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds)
        {
            var manufacturersInDb = await DbPolicy.ExecuteAsync(() =>
                _manufacturerRepository.GetRangeAsync(m => manufacturerIds.Contains(m.Id)));
            var manufacturers = Mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }

        public async Task<Result<ManufacturerDto>> GetAsync(string id)
        {
            var cacheKey = $"Manufacturer-{id}";
            var cache = await CachingPolicy.ExecuteAsync(() => DistributedCache.GetStringAsync(cacheKey));
            ManufacturerDto manufacturer;

            if (cache.TryGetValue<Manufacturer>(out var cachedManufacturer))
            {
                manufacturer = Mapper.Map<ManufacturerDto>(cachedManufacturer);

                return Result<ManufacturerDto>.Success(manufacturer);
            }

            var manufacturerInDb = await DbPolicy.ExecuteAsync(() => _manufacturerRepository.GetAsync(m => m.Id == id));
            if (manufacturerInDb is not null)
            {
                await CachingPolicy.ExecuteAsync(() =>
                    DistributedCache.SetCacheAsync(cacheKey, manufacturerInDb, _manufacturerSettings));
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

            await CachingPolicy.ExecuteAsync(() =>
                DistributedCache.SetCacheAsync(cacheKey, manufacturerToDb, _manufacturerSettings));
            await FileService.WriteToFileAsync(manufacturerToDb, _path, cacheKey);

            await DbPolicy.ExecuteAsync(() => _manufacturerRepository.CreateAsync(manufacturerToDb));
            await RabbitPolicy.ExecuteAsync(() => Bus.PubSub.PublishAsync(new CreatedManufacturer(manufacturerToDb)));
            await RabbitPolicy.ExecuteAsync(() => Bus.PubSub.PublishAsync(log));
            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<ManufacturerDto>> UpdateAsync(string manufacturerId, ManufacturerModelDto manufacturer,
            string userName)
        {
            var cacheKey = $"Manufacturer-{manufacturerId}";
            Manufacturer manufacturerInDb;
            if (!await CachingPolicy.ExecuteAsync(() => DistributedCache.IsExistsAsync(cacheKey)))
            {
                manufacturerInDb =
                    await DbPolicy.ExecuteAsync(() => _manufacturerRepository.GetAsync(m => m.Id == manufacturerId)) ??
                    await FileService.ReadFromFileAsync<Manufacturer>(_path, cacheKey);
                CheckForNull(manufacturerInDb);
            }

            var manufacturerDto = Mapper.Map<ManufacturerDto>(manufacturer) with {Id = manufacturerId};
            manufacturerInDb = Mapper.Map<Manufacturer>(manufacturerDto);
            LogDto log =
                new(Guid.NewGuid().ToString(), userName, "edited manufacturer", JsonSerializer.Serialize(
                    manufacturerDto,
                    JsonSerializerOptions), DateTime.UtcNow);

            await DbPolicy.ExecuteAsync(() =>
                _manufacturerRepository.UpdateAsync(m => m.Id == manufacturerInDb.Id, manufacturerInDb));
            await RabbitPolicy.ExecuteAsync(() => Bus.PubSub.PublishAsync(new UpdatedManufacturer(manufacturerInDb)));
            await RabbitPolicy.ExecuteAsync(() => Bus.PubSub.PublishAsync(log));
            await CachingPolicy.ExecuteAsync(() => DistributedCache.UpdateAsync(cacheKey, manufacturerInDb));
            await FileService.WriteToFileAsync(manufacturerInDb, _path, cacheKey);

            return Result<ManufacturerDto>.Success(manufacturerDto);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"Manufacturer-{id}";
            if (!await CachingPolicy.ExecuteAsync(() => DistributedCache.IsExistsAsync(cacheKey)))
            {
                var manufacturerInDb =
                    await DbPolicy.ExecuteAsync(() => _manufacturerRepository.GetAsync(m => m.Id == id));
                CheckForNull(manufacturerInDb);
            }

            await RabbitPolicy.ExecuteAsync(() => Bus.PubSub.PublishAsync(new DeletedManufacturer(id)));
            await DbPolicy.ExecuteAsync(() => _manufacturerRepository.DeleteAsync(m => m.Id == id));
            await FileService.DeleteFileAsync(_path, cacheKey);

            await CachingPolicy.ExecuteAsync(() => DistributedCache.RemoveAsync(cacheKey));

            return Result<object>.Success();
        }
    }
}