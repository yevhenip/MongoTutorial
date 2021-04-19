using System.Collections.Generic;
using System.Threading.Tasks;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Core.Interfaces.Services
{
    public interface IManufacturerService
    {
        Task<Result<List<ManufacturerDto>>> GetAllAsync();

        Task<Result<PageDataDto<ManufacturerDto>>> GetPageAsync(int page, int pageSize);

        Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds);

        Task<Result<ManufacturerDto>> GetAsync(string id);

        Task<Result<ManufacturerDto>> CreateAsync(ManufacturerModelDto manufacturer, string userName);

        Task<Result<ManufacturerDto>> UpdateAsync(string manufacturerId, ManufacturerModelDto manufacturer,
            string userName);

        Task<Result<object>> DeleteAsync(string id);
    }
}