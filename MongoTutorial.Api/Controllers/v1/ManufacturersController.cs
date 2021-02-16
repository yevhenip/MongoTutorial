using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Api.Models.Manufacturer;
using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.Controllers.v1
{
    public class ManufacturersController : ApiControllerBase
    {
        private readonly IManufacturerService _manufacturerService;
        private readonly IMapper _mapper;

        public ManufacturersController(IManufacturerService manufacturerService, IMapper mapper)
        {
            _manufacturerService = manufacturerService;
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetManufacturersAsync()
        {
            var result = await _manufacturerService.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpGet("{manufacturersId}")]
        public async Task<IActionResult> GetManufacturersByIdAsync([FromRoute] string manufacturersId)
        {
            var result = await _manufacturerService.GetAsync(manufacturersId);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateManufacturersAsync([FromBody] ManufacturerModel manufacturers)
        {
            var manufacturersToDb = _mapper.Map<ManufacturerDto>(manufacturers);
            var result = await _manufacturerService.CreateAsync(manufacturersToDb);
            return Ok(result.Data);
        }

        [HttpPut("{manufacturersId}")]
        public async Task<IActionResult> UpdateManufacturersAsync([FromRoute] string manufacturersId,
            [FromBody] ManufacturerModel manufacturers)
        {
            var manufacturersToDb = _mapper.Map<ManufacturerDto>(manufacturers) with {Id = manufacturersId};
            var result = await _manufacturerService.UpdateAsync(manufacturersToDb);
            return Ok(result.Data);
        }

        [HttpDelete("{manufacturersId}")]
        public async Task<IActionResult> DeleteManufacturersAsync([FromRoute] string manufacturersId)
        {
            var result = await _manufacturerService.DeleteAsync(manufacturersId);
            return Ok(result.Data);
        }
    }
}