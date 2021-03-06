using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Core.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Extensions;
using Warehouse.Core.Interfaces.Messaging.Sender;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Customers.Business
{
    public class CustomerService : ServiceBase<Customer>, ICustomerService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Customers\";
        private readonly CacheCustomerSettings _customerSettings;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISender _sender;

        public CustomerService(IOptions<CacheCustomerSettings> customerSettings, ICustomerRepository customerRepository,
            IDistributedCache distributedCache, IMapper mapper, ISender sender) : base(distributedCache, mapper)
        {
            _customerSettings = customerSettings.Value;
            _customerRepository = customerRepository;
            _sender = sender;
        }


        public async Task<Result<List<CustomerDto>>> GetAllAsync()
        {
            var customersInDb = await _customerRepository.GetAllAsync();
            var customers = Mapper.Map<List<CustomerDto>>(customersInDb);

            return Result<List<CustomerDto>>.Success(customers);
        }

        public async Task<Result<CustomerDto>> GetAsync(string id)
        {
            var cacheKey = $"Customer-{id}";
            var cache = await DistributedCache.GetStringAsync(cacheKey);
            CustomerDto customer;

            if (cache.TryGetValue<Customer>(out var cachedCustomer))
            {
                customer = Mapper.Map<CustomerDto>(cachedCustomer);

                return Result<CustomerDto>.Success(customer);
            }

            var customerInDb = await _customerRepository.GetAsync(id);
            if (customerInDb is not null)
            {
                await DistributedCache.SetCacheAsync(cacheKey, customerInDb, _customerSettings);
                customer = Mapper.Map<CustomerDto>(customerInDb);

                return Result<CustomerDto>.Success(customer);
            }

            customerInDb = await FileExtensions.ReadFromFileAsync<Customer>(_path, cacheKey);
            CheckForNull(customerInDb);

            await DistributedCache.SetCacheAsync(cacheKey, customerInDb, _customerSettings);
            customer = Mapper.Map<CustomerDto>(customerInDb);

            return Result<CustomerDto>.Success(customer);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"Customer-{id}";
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                var customerInDb = await _customerRepository.GetAsync(id);
                CheckForNull(customerInDb);
            }

            await _sender.SendMessage(id, "DeleteCustomerQueue");
            await DistributedCache.RemoveAsync(cacheKey);
            await FileExtensions.DeleteFileAsync(_path, cacheKey);

            return Result<object>.Success();
        }

        public async Task<Result<CustomerDto>> CreateAsync(CustomerDto customer)
        {
            
            var customerDto = Mapper.Map<CustomerDto>(customer);
            var customerToDb = Mapper.Map<Customer>(customerDto);

            var cacheKey = $"Customer-{customerToDb.Id}";
            await DistributedCache.SetCacheAsync(cacheKey, customerToDb, _customerSettings);
            await customerToDb.WriteToFileAsync(_path, cacheKey);

            await _customerRepository.CreateAsync(customerToDb);
            await _sender.SendMessage(customerToDb, "CreateCustomerQueue");
            return Result<CustomerDto>.Success(customerDto);
        }
    }
}