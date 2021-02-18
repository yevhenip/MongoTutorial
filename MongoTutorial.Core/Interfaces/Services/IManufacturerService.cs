using System.Collections.Generic;
using System.Threading.Tasks;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Manufacturer;

namespace MongoTutorial.Core.Interfaces.Services
{
    public interface IManufacturerService
    {
        Task<Result<List<ManufacturerDto>>> GetAllAsync();

        Task<Result<List<ManufacturerDto>>> GetRangeAsync(IEnumerable<string> manufacturerIds);

        Task<Result<ManufacturerDto>> GetAsync(string id);

        Task<Result<ManufacturerDto>> CreateAsync(ManufacturerModelDto manufacturer);

        Task<Result<ManufacturerDto>> UpdateAsync(string id, ManufacturerModelDto manufacturer);

        Task<Result<object>> DeleteAsync(string id);
    }
}