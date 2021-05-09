using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Customers.Commands
{
    public record GetCustomerCommand(string Id) : IRequest<Result<CustomerDto>>;

    public class GetCustomerCommandHandler : IRequestHandler<GetCustomerCommand, Result<CustomerDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ICustomerRepository _customerRepository;
        private readonly CacheCustomerSettings _customerSettings;

        public GetCustomerCommandHandler(IMapper mapper, ICacheService cacheService,
            ICustomerRepository customerRepository, IOptions<CacheCustomerSettings> customerSettings)
        {
            _mapper = mapper;
            _cacheService = cacheService;
            _customerRepository = customerRepository;
            _customerSettings = customerSettings.Value;
        }

        public async Task<Result<CustomerDto>> Handle(GetCustomerCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Customer-{request.Id}";
            var cache = await _cacheService.GetStringAsync(cacheKey);
            CustomerDto customer;

            if (cache.TryGetValue<Customer>(out var cachedCustomer))
            {
                customer = _mapper.Map<CustomerDto>(cachedCustomer);

                return Result<CustomerDto>.Success(customer);
            }

            var customerInDb = await _customerRepository.GetAsync(c => c.Id == request.Id);

            customerInDb.CheckForNull();
            await _cacheService.SetCacheAsync(cacheKey, customerInDb, _customerSettings);

            customer = _mapper.Map<CustomerDto>(customerInDb);

            return Result<CustomerDto>.Success(customer);
        }
    }
}