using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Log;

namespace Warehouse.Core.Interfaces.Services
{
    public interface ILogService
    {
        Task<Result<List<LogDto>>> GetAllAsync();
        
        Task<Result<List<LogDto>>> GetActualAsync();

        Task<Result<LogDto>> GetAsync(string id);
        
        Task CreateAsync(LogDto log);
    }
}