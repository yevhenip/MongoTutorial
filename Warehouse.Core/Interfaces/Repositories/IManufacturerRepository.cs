using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IManufacturerRepository
    {
        Task<List<Manufacturer>> GetAllAsync();

        Task<Manufacturer> GetAsync(string id);

        Task CreateAsync(Manufacturer manufacturer);

        Task UpdateAsync(Manufacturer manufacturer);

        Task DeleteAsync(string id);

        Task<List<Manufacturer>> GetRangeAsync(IEnumerable<string> manufacturerIds);
    }
}