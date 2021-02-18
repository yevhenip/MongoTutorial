using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Core.DTO.Product;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.Controllers.v1
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

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string productId)
        {
            var result = await _productService.GetAsync(productId);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductModelDto product)
        {
            var result = await _productService.CreateAsync(product);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string productId,
            [FromBody] ProductModelDto product)
        {
            var result = await _productService.UpdateAsync(productId, product);
            return Ok(result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string productId)
        {
            var result = await _productService.DeleteAsync(productId);
            return Ok(result.Data);
        }
    }
}