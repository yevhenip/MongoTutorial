using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Controllers.v1;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Manufacturers.Controllers.v1
{
    [Authorize]
    public class ManufacturersController : ApiControllerBase
    {
        private readonly IManufacturerService _manufacturerService;

        public ManufacturersController(IManufacturerService manufacturerService)
        {
            _manufacturerService = manufacturerService;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _manufacturerService.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpGet("{manufacturerId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] string manufacturerId)
        {
            var result = await _manufacturerService.GetAsync(manufacturerId);
            return Ok(result.Data);
        }
        
        [HttpGet("{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPageAsync([FromRoute] int page, [FromRoute] int pageSize)
        {
            var result = await _manufacturerService.GetPageAsync(page, pageSize);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ManufacturerModelDto manufacturers)
        {
            var result = await _manufacturerService.CreateAsync(manufacturers, UserName);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{manufacturerId:guid}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string manufacturerId,
            [FromBody] ManufacturerModelDto manufacturers)
        {
            var result = await _manufacturerService.UpdateAsync(manufacturerId, manufacturers, UserName);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{manufacturerId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string manufacturerId)
        {
            var result = await _manufacturerService.DeleteAsync(manufacturerId);
            return Ok(result.Data);
        }
    }
}