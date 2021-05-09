using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using ISender = Warehouse.Core.Interfaces.Services.ISender;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Warehouse.Api.Customers.Commands
{
    public record CreateCustomerCommand(CustomerDto Customer, string UserName) : IRequest<Result<CustomerDto>>;

    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISender _sender;

        public CreateCustomerCommandHandler(IMapper mapper, ISender sender, ICacheService cacheService,
            ICustomerRepository customerRepository, IOptions<CacheCustomerSettings> customerSettings)
        {
            _mapper = mapper;
            _cacheService = cacheService;
            _customerRepository = customerRepository;
            _sender = sender;
            _customerSettings = customerSettings.Value;
        }

        private readonly CacheCustomerSettings _customerSettings;

        public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            var customerToDb = _mapper.Map<Customer>(request.Customer);

            var cacheKey = $"Customer-{customerToDb.Id}";
            LogDto log =
                new(Guid.NewGuid().ToString(), request.UserName, "added customer", JsonSerializer.Serialize(
                    customerToDb,
                    CommandExtensions.JsonSerializerOptions), DateTime.UtcNow);

            await _cacheService.SetCacheAsync(cacheKey, customerToDb, _customerSettings);

            await _customerRepository.CreateAsync(customerToDb);
            await _sender.PublishAsync(new CreatedCustomer(customerToDb), cancellationToken);
            await _sender.PublishAsync(log, cancellationToken);
            return Result<CustomerDto>.Success(request.Customer with {Id = customerToDb.Id});
        }
    }
}