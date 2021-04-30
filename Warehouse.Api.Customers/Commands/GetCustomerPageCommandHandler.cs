using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Interfaces.Repositories;

namespace Warehouse.Api.Customers.Commands
{
    public record GetCustomerPageCommand(int Page, int PageSize) : IRequest<Result<PageDataDto<CustomerDto>>>;

    public class
        GetCustomerPageCommandHandler : IRequestHandler<GetCustomerPageCommand, Result<PageDataDto<CustomerDto>>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public GetCustomerPageCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<Result<PageDataDto<CustomerDto>>> Handle(GetCustomerPageCommand request,
            CancellationToken cancellationToken)
        {
            var customersInDb = await _customerRepository.GetPageAsync(request.Page, request.PageSize);
            var count = await _customerRepository.GetCountAsync(_ => true);
            var customers = _mapper.Map<List<CustomerDto>>(customersInDb);
            PageDataDto<CustomerDto> pageData = new(customers, count);

            return Result<PageDataDto<CustomerDto>>.Success(pageData);
        }
    }
}