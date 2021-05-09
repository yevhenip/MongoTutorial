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
    public record UpdateManufacturerCommand(string ManufacturerId, ManufacturerModelDto Manufacturer,
        string UserName) : IRequest<Result<ManufacturerDto>>;

    public class UpdateManufacturerCommandHandler : IRequestHandler<UpdateManufacturerCommand, Result<ManufacturerDto>>
    {
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly ISender _sender;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public UpdateManufacturerCommandHandler(IOptions<CacheManufacturerSettings> manufacturerSettings,
            ISender sender, IManufacturerRepository manufacturerRepository, ICacheService cacheService, IMapper mapper)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _sender = sender;
            _manufacturerRepository = manufacturerRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<Result<ManufacturerDto>> Handle(UpdateManufacturerCommand request,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"Manufacturer-{request.ManufacturerId}";
            Manufacturer manufacturerInDb;
            if (!await _cacheService.IsExistsAsync(cacheKey))
            {
                manufacturerInDb = await _manufacturerRepository.GetAsync(m => m.Id == request.ManufacturerId);
                manufacturerInDb.CheckForNull();
            }

            var manufacturerDto = _mapper.Map<ManufacturerDto>(request.Manufacturer) with {Id = request.ManufacturerId};
            manufacturerInDb = _mapper.Map<Manufacturer>(manufacturerDto);
            LogDto log = new(Guid.NewGuid().ToString(), request.UserName, "edited manufacturer",
                JsonSerializer.Serialize(manufacturerDto, CommandExtensions.JsonSerializerOptions), DateTime.UtcNow);

            await _manufacturerRepository.UpdateAsync(m => m.Id == manufacturerInDb.Id, manufacturerInDb);
            await _sender.PublishAsync(new UpdatedManufacturer(manufacturerInDb), cancellationToken);
            await _sender.PublishAsync(log, cancellationToken);
            await _cacheService.UpdateAsync(cacheKey, manufacturerInDb, _manufacturerSettings);

            return Result<ManufacturerDto>.Success(manufacturerDto);
        }
    }
}