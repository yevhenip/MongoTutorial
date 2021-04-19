﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync();

        public Task<List<Product>> GetPageAsync(int page, int pageSize);

        public Task<long> GetCountAsync();
        
        Task<Product> GetAsync(string id);

        Task CreateAsync(Product product);

        Task UpdateAsync(Product product);

        Task DeleteAsync(string id);

        Task<List<Product>> GetRangeByManufacturerId(string manufacturerId);
        Task<List<Product>> GetByCustomerId(string customerId);
    }
}