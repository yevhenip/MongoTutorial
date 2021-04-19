using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IManufacturerService
    {
        /// <summary>
        /// Gets all manufacturers
        /// </summary>
        /// <returns>List of manufacturers</returns>
        Task<Result<List<ManufacturerDto>>> GetAllAsync();

        Task<Result<PageDataDto<ManufacturerDto>>> GetPageAsync(int page, int pageSize);

        Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds);

        /// <summary>
        /// Tries to get manufacturer firstly from cache, then from database, then from file. If manufacturer still null throws an exception
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Manufacturer</returns>
        Task<Result<ManufacturerDto>> GetAsync(string id);

        /// <summary>
        /// Creates, sets to cache, file system and sends a message with manufacturer
        /// </summary>
        /// <param name="manufacturer"></param>
        /// <param name="userName"></param>
        /// <returns>Created manufacturer</returns>
        Task<Result<ManufacturerDto>> CreateAsync(ManufacturerModelDto manufacturer, string userName);

        /// <summary>
        /// Updates manufacturer in database, cache, filesystem and sends message with manufacturer
        /// </summary>
        /// <param name="manufacturerId"></param>
        /// <param name="manufacturer"></param>
        /// <param name="userName"></param>
        /// <returns>Updated manufacturer</returns>
        Task<Result<ManufacturerDto>> UpdateAsync(string manufacturerId, ManufacturerModelDto manufacturer,
            string userName);

        /// <summary>
        /// Deletes manufacturer from cache, file system, database and sends a message with manufacturer id
        /// </summary>
        /// <param name="id"></param>
        Task<Result<object>> DeleteAsync(string id);
    }
}