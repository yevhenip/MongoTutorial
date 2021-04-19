using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<Result<List<CustomerDto>>> GetAllAsync();
        
        Task<Result<PageDataDto<CustomerDto>>> GetPageAsync(int page, int pageSize);

        Task<Result<CustomerDto>> GetAsync(string id);

        Task<Result<object>> DeleteAsync(string id);
        
        Task<Result<CustomerDto>> CreateAsync(CustomerDto customer, string userName);
    }
}