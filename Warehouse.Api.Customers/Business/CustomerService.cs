using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Api.Business;
using Warehouse.Api.Extensions;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Warehouse.Api.Customers.Business
{
    public class CustomerService : ServiceBase<Customer>, ICustomerService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Customers\";
        private readonly CacheCustomerSettings _customerSettings;
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(IOptions<CacheCustomerSettings> customerSettings, ICustomerRepository customerRepository,
            IDistributedCache distributedCache, IMapper mapper, IBus bus, IFileService fileService) :
            base(mapper, distributedCache, bus, fileService)
        {
            _customerSettings = customerSettings.Value;
            _customerRepository = customerRepository;
        }


        public async Task<Result<List<CustomerDto>>> GetAllAsync()
        {
            var customersInDb = await _customerRepository.GetRangeAsync(_ => true);
            var customers = Mapper.Map<List<CustomerDto>>(customersInDb);

            return Result<List<CustomerDto>>.Success(customers);
        }

        public async Task<Result<PageDataDto<CustomerDto>>> GetPageAsync(int page, int pageSize)
        {
            var customersInDb = await _customerRepository.GetPageAsync(page, pageSize);
            var count = await _customerRepository.GetCountAsync(_ => true);
            var customers = Mapper.Map<List<CustomerDto>>(customersInDb);
            PageDataDto<CustomerDto> pageData = new(customers, count);

            return Result<PageDataDto<CustomerDto>>.Success(pageData);
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

            var customerInDb = await _customerRepository.GetAsync(c => c.Id == id);
            if (customerInDb is not null)
            {
                await DistributedCache.SetCacheAsync(cacheKey, customerInDb, _customerSettings);
                customer = Mapper.Map<CustomerDto>(customerInDb);

                return Result<CustomerDto>.Success(customer);
            }

            customerInDb = await FileService.ReadFromFileAsync<Customer>(_path, cacheKey);
            CheckForNull(customerInDb);

            customer = Mapper.Map<CustomerDto>(customerInDb);

            return Result<CustomerDto>.Success(customer);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"Customer-{id}";
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                var customerInDb = await _customerRepository.GetAsync(c => c.Id == id);
                CheckForNull(customerInDb);
            }

            await Bus.PubSub.PublishAsync(new DeletedCustomer(id));
            await DistributedCache.RemoveAsync(cacheKey);
            await FileService.DeleteFileAsync(_path, cacheKey);
            await _customerRepository.DeleteAsync(c => c.Id == id);

            return Result<object>.Success();
        }

        public async Task<Result<CustomerDto>> CreateAsync(CustomerDto customer, string userName)
        {
            var customerToDb = Mapper.Map<Customer>(customer);

            var cacheKey = $"Customer-{customerToDb.Id}";
            LogDto log =
                new(Guid.NewGuid().ToString(), userName, "added customer", JsonSerializer.Serialize(customerToDb,
                    JsonSerializerOptions), DateTime.UtcNow);

            await DistributedCache.SetCacheAsync(cacheKey, customerToDb, _customerSettings);
            await FileService.WriteToFileAsync(customerToDb, _path, cacheKey);

            await _customerRepository.CreateAsync(customerToDb);
            await Bus.PubSub.PublishAsync(new CreatedCustomer(customerToDb));
            await Bus.PubSub.PublishAsync(log);
            return Result<CustomerDto>.Success(customer with {Id = customerToDb.Id});
        }
    }
}