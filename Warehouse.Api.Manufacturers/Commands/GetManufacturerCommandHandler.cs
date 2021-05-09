using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Manufacturers.Commands
{
    public record GetManufacturerCommand(string Id) : IRequest<Result<ManufacturerDto>>;

    public class GetManufacturerCommandHandler : IRequestHandler<GetManufacturerCommand, Result<ManufacturerDto>>
    {
        private readonly CacheManufacturerSettings _manufacturerSettings;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public GetManufacturerCommandHandler(IMapper mapper, ICacheService cacheService,
            IOptions<CacheManufacturerSettings> manufacturerSettings, IManufacturerRepository manufacturerRepository)
        {
            _manufacturerSettings = manufacturerSettings.Value;
            _manufacturerRepository = manufacturerRepository;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task<Result<ManufacturerDto>> Handle(GetManufacturerCommand request,
            CancellationToken cancellationToken)
        {
            var cacheKey = $"Manufacturer-{request.Id}";
            var cache = await _cacheService.GetStringAsync(cacheKey);
            ManufacturerDto manufacturer;

            if (cache.TryGetValue<Manufacturer>(out var cachedManufacturer))
            {
                manufacturer = _mapper.Map<ManufacturerDto>(cachedManufacturer);

                return Result<ManufacturerDto>.Success(manufacturer);
            }

            var manufacturerInDb = await _manufacturerRepository.GetAsync(m => m.Id == request.Id);
            manufacturerInDb.CheckForNull();
            await _cacheService.SetCacheAsync(cacheKey, manufacturerInDb, _manufacturerSettings);

            manufacturer = _mapper.Map<ManufacturerDto>(manufacturerInDb);

            return Result<ManufacturerDto>.Success(manufacturer);
        }
    }
}