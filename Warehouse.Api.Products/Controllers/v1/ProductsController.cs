using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Api.Controllers.v1;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Controllers.v1
{
    [Authorize]
    public class ProductsController : ApiControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _productService.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpGet("{page:int}/{pageSize:int}")]
        public async Task<IActionResult> GetPageAsync([FromRoute] int page, [FromRoute] int pageSize)
        {
            var result = await _productService.GetPageAsync(page, pageSize);
            return Ok(result.Data);
        }

        [HttpGet("{productId:guid}")]
        public async Task<IActionResult> GetAsync([FromRoute] string productId)
        {
            var result = await _productService.GetAsync(productId);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("export")]
        public async Task<IActionResult> GetExportFileAsync()
        {
            var result = await _productService.GetExportFileAsync();
            return File(result.Data, "text/csv", "products.csv");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductModelDto product)
        {
            var result = await _productService.CreateAsync(product, UserName);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{productId:guid}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string productId,
            [FromBody] ProductModelDto product)
        {
            var result = await _productService.UpdateAsync(productId, product, UserName);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string productId)
        {
            var result = await _productService.DeleteAsync(productId);
            return Ok(result.Data);
        }
    }
}