using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Base;
using Warehouse.Api.Products.Commands;
using Warehouse.Core;
using Warehouse.Core.DTO.Product;

namespace Warehouse.Api.Products.Controllers.v1
{
    [Authorize]
    public class ProductsController : ApiControllerBase
    {
        public ProductsController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await Mediator.Send(new GetProductsCommand(_ => true));
            return Ok(result.Data);
        }


        [HttpGet("{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPageAsync([FromRoute] int page, [FromRoute] int pageSize)
        {
            var result = await Mediator.Send(new GetProductsPageCommand(page, pageSize));
            return Ok(result.Data);
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] string productId)
        {
            var result = await Mediator.Send(new GetProductCommand(productId));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("export")]
        public async Task<IActionResult> GetExportFileAsync()
        {
            var result = await Mediator.Send(new GetExportFileCommand());
            return File(result.Data, "text/csv", "products.csv");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("import")]
        public async Task<IActionResult> ImportProductsFromFileAsync([FromForm]FileModel file)
        {
            var result = await Mediator.Send(new ImportProductsFromFileCommand(file.File, UserName));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductModelDto product)
        {
            var result = await Mediator.Send(new CreateProductCommand(product, UserName));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{productId:guid}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string productId,
            [FromBody] ProductModelDto product)
        {
            var result = await Mediator.Send(new UpdateProductCommand(productId, product, UserName));
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string productId)
        {
            var result = await Mediator.Send(new DeleteProductCommand(productId));
            return Ok(result.Data);
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("group")]
        public async Task<IActionResult> GetGroupedDataAsync()
        {
            var result = await Mediator.Send(new GetGroupedDataCommand());
            return Ok(result.Data);
        }
    }
}