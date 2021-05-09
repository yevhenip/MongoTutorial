using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using ISender = Warehouse.Core.Interfaces.Services.ISender;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Warehouse.Api.Manufacturers.Commands
{
    public record CreateManufacturerCommand(ManufacturerModelDto Manufacturer, string UserName)
        : IRequest<Result<ManufacturerDto>>;

    public class CreateManufacturerCommandHandler : IRequestHandler<CreateManufacturerCommand, Result<ManufacturerDto>>
    {
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly ISender _sender;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public CreateManufacturerCommandHandler(ISender sender, ICacheService cacheService, IMapper mapper,
            IManufacturerRepository manufacturerRepository, IOptions<CacheManufacturerSettings> manufacturerSettings)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _sender = sender;
            _manufacturerRepository = manufacturerRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<Result<ManufacturerDto>> Handle(CreateManufacturerCommand request,
            CancellationToken cancellationToken)
        {
            var manufacturerDto = _mapper.Map<ManufacturerDto>(request.Manufacturer);
            var manufacturerToDb = _mapper.Map<Manufacturer>(manufacturerDto);

            var cacheKey = $"Manufacturer-{manufacturerToDb.Id}";
            LogDto log = new(Guid.NewGuid().ToString(), request.UserName, "added manufacturer",
                JsonSerializer.Serialize(manufacturerDto, CommandExtensions.JsonSerializerOptions), DateTime.UtcNow);

            await _cacheService.SetCacheAsync(cacheKey, manufacturerToDb, _manufacturerSettings);

            await _manufacturerRepository.CreateAsync(manufacturerToDb);
            await _sender.PublishAsync(new CreatedManufacturer(manufacturerToDb), cancellationToken);
            await _sender.PublishAsync(log, cancellationToken);
            return Result<ManufacturerDto>.Success(manufacturerDto);
        }
    }
}