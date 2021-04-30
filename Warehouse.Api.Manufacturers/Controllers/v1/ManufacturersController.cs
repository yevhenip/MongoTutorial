using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Base;
using Warehouse.Api.Manufacturers.Commands;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Manufacturers.Controllers.v1
{
    [Authorize]
    public class ManufacturersController : ApiControllerBase
    {
        public ManufacturersController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await Mediator.Send(new GetManufacturersCommand(_ => true));
            return Ok(result.Data);
        }

        [HttpGet("{manufacturerId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] string manufacturerId)
        {
            var result = await Mediator.Send(new GetManufacturerCommand(manufacturerId));
            return Ok(result.Data);
        }

        [HttpGet("{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPageAsync([FromRoute] int page, [FromRoute] int pageSize)
        {
            var result = await Mediator.Send(new GetManufacturersPageCommand(page, pageSize));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ManufacturerModelDto manufacturers)
        {
            var result = await Mediator.Send(new CreateManufacturerCommand(manufacturers, UserName));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{manufacturerId:guid}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string manufacturerId,
            [FromBody] ManufacturerModelDto manufacturers)
        {
            var result = await Mediator.Send(new UpdateManufacturerCommand(manufacturerId, manufacturers, UserName));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{manufacturerId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string manufacturerId)
        {
            var result = await Mediator.Send(new DeleteManufacturerCommand(manufacturerId));
            return Ok(result.Data);
        }
    }
}