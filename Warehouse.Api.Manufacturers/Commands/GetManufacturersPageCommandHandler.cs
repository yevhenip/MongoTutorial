using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Manufacturers.Commands
{
    public record GetManufacturersPageCommand(int Page, int PageSize) : IRequest<Result<PageDataDto<ManufacturerDto>>>;

    public class
        GetManufacturersPageCommandHandler : IRequestHandler<GetManufacturersPageCommand,
            Result<PageDataDto<ManufacturerDto>>>
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;

        public GetManufacturersPageCommandHandler(IManufacturerRepository manufacturerRepository, IMapper mapper)
        {
            _manufacturerRepository = manufacturerRepository;
            _mapper = mapper;
        }

        public async Task<Result<PageDataDto<ManufacturerDto>>> Handle(GetManufacturersPageCommand request,
            CancellationToken cancellationToken)
        {
            var manufacturersInDb =
                await _manufacturerRepository.GetPageAsync(request.Page, request.PageSize);
            var count = await _manufacturerRepository.GetCountAsync(_ => true);
            var manufacturers = _mapper.Map<List<ManufacturerDto>>(manufacturersInDb);
            PageDataDto<ManufacturerDto> pageData = new(manufacturers, count);

            return Result<PageDataDto<ManufacturerDto>>.Success(pageData);
        }
    }
}