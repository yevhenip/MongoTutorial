using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface ILogRepository
    {
        Task<List<Log>> GetAllAsync();
        
        Task<List<Log>> GetActualAsync();

        Task<Log> GetAsync(string id);

        Task CreateAsync(Log log);

        Task DeleteAsync(string id);
    }
}