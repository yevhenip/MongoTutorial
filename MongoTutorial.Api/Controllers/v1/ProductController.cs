using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.Controllers.v1
{
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
                return BadRequest("Product with such id not exists");
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductDto product)
        {
            product.Id = Guid.NewGuid().ToString();
            await _productService.CreateProductAsync(product);
            return Ok(product);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync([FromRoute] string productId, [FromBody] ProductDto product)
        {
            var productFromDb = await _productService.GetProductByIdAsync(productId);
            if (productFromDb.Id != productId)
            {
                return BadRequest("Illegal specified id");
            }

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