using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Domain;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IManufacturerRepository
    {
        /// <summary>
        /// Gets all manufacturers in database
        /// </summary>
        /// <returns>List of manufacturers</returns>
        Task<List<Manufacturer>> GetAllAsync();
        
        public Task<List<Manufacturer>> GetPageAsync(int page, int pageSize);

        public Task<long> GetCountAsync();

        /// <summary>
        /// Returns manufacturer in database based on provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Manufacturer</returns>
        Task<Manufacturer> GetAsync(string id);

        /// <summary>
        /// Sets new manufacturer
        /// </summary>
        /// <param name="manufacturer"></param>
        Task CreateAsync(Manufacturer manufacturer);

        /// <summary>
        /// Updates manufacturer in database
        /// </summary>
        /// <param name="manufacturer"></param>
        Task UpdateAsync(Manufacturer manufacturer);

        /// <summary>
        /// Delete manufacturer from database
        /// </summary>
        /// <param name="id"></param>
        Task DeleteAsync(string id);

        /// <summary>
        /// Gets manufacturers in database based on provided ids
        /// </summary>
        /// <param name="manufacturerIds"></param>
        /// <returns>List of manufacturers</returns>
        Task<List<Manufacturer>> GetRangeAsync(IEnumerable<string> manufacturerIds);
    }
}