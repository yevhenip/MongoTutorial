using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Warehouse.Api.Common;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Customers.Commands
{
    public record GetCustomersCommand(Expression<Func<Customer, bool>> Predicate) : IRequest<Result<List<CustomerDto>>>;

    public class GetCustomersCommandHandler : IRequestHandler<GetCustomersCommand, Result<List<CustomerDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;

        public GetCustomersCommandHandler(IMapper mapper, ICustomerRepository customerRepository)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        public async Task<Result<List<CustomerDto>>> Handle(GetCustomersCommand request,
            CancellationToken cancellationToken)
        {
            var customersInDb = await _customerRepository.GetRangeAsync(request.Predicate);
            var customers = _mapper.Map<List<CustomerDto>>(customersInDb);

            return Result<List<CustomerDto>>.Success(customers);
        }
    }
}