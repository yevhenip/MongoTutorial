using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Manufacturers.Commands
{
    public record GetManufacturersCommand
        (Expression<Func<Manufacturer, bool>> Predicate) : IRequest<Result<List<ManufacturerDto>>>;

    public class
        GetManufacturersCommandHandler : IRequestHandler<GetManufacturersCommand, Result<List<ManufacturerDto>>>
    {
        private readonly IManufacturerRepository _manufacturerRepository;
        private readonly IMapper _mapper;

        public GetManufacturersCommandHandler(IManufacturerRepository manufacturerRepository, IMapper mapper)
        {
            _manufacturerRepository = manufacturerRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ManufacturerDto>>> Handle(GetManufacturersCommand request,
            CancellationToken cancellationToken)
        {
            var manufacturersInDb = await _manufacturerRepository.GetRangeAsync(request.Predicate);
            var manufacturers = _mapper.Map<List<ManufacturerDto>>(manufacturersInDb);

            return Result<List<ManufacturerDto>>.Success(manufacturers);
        }
    }
}