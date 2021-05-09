using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using ISender = Warehouse.Core.Interfaces.Services.ISender;

namespace Warehouse.Api.Manufacturers.Commands
{
    public record DeleteManufacturerCommand(string Id) : IRequest<Result<object>>;

    public class DeleteManufacturerCommandHandler : IRequestHandler<DeleteManufacturerCommand, Result<object>>
    {
        private readonly ISender _sender;
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly ICacheService _cacheService;

        public DeleteManufacturerCommandHandler(ISender sender, IManufacturerRepository manufacturerRepository,
            ICacheService cacheService)
        {
            _sender = sender;
            _manufacturerRepository = manufacturerRepository;
            _cacheService = cacheService;
        }

        public async Task<Result<object>> Handle(DeleteManufacturerCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Manufacturer-{request.Id}";
            if (!await _cacheService.IsExistsAsync(cacheKey))
            {
                var manufacturerInDb = await _manufacturerRepository.GetAsync(m => m.Id == request.Id);
                manufacturerInDb.CheckForNull();
            }

            await _sender.PublishAsync(new DeletedManufacturer(request.Id), cancellationToken);
            await _manufacturerRepository.DeleteAsync(m => m.Id == request.Id);

            await _cacheService.RemoveAsync(cacheKey);

            return Result<object>.Success();
        }
    }
}