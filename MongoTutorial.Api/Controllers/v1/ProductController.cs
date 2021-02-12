using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.Controllers.v1
{
    // Probably add some error handling middleware
    // Rename to ProductsController
    public class ProductController : ApiControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var products = await _productService.GetProductsAsync();
            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] string productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                // If something is not found, better return NotFound("...")
                // look up about BadRequest in the internet
                return BadRequest("Product with such id does not exist");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductDto product)
        {
            // Move ID assignment to service/repo
            product.Id = Guid.NewGuid().ToString();
            // Should we check that required fields are not empty/not null?
            await _productService.CreateProductAsync(product);
            return Ok(product);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync([FromRoute] string productId, [FromBody] ProductDto product)
        {
            var productFromDb = await _productService.GetProductByIdAsync(productId);
            // does this check make sense? Maybe better to handle not found?
            if (productFromDb.Id != productId)
            {
                return BadRequest("Specified id is incorrect/not found");
            }

            // Should we check that required fields are not empty/not null?
            // FluentValidation

            // create new request model (without id)
            product.Id = productId;
            await _productService.UpdateProductAsync(product);
            return Ok(product);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] string productId)
        {
            await _productService.DeleteProductAsync(productId);
            return Ok();
        }
    }
}