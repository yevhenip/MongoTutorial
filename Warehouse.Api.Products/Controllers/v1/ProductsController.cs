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

        /// <summary>
        /// Gets all products
        /// </summary>
        /// <returns>List of products</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _productService.GetAllAsync();
            return Ok(result.Data);
        }

        /// <summary>
        /// Gets product based on provided id
        /// </summary>
        /// <param name="productId"></param>
        /// <returns>Product</returns>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetAsync([FromRoute] string productId)
        {
            var result = await _productService.GetAsync(productId);
            return Ok(result.Data);
        }

        /// <summary>
        /// Creates product
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Created product</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ProductModelDto product)
        {
            var result = await _productService.CreateAsync(product);
            return Ok(result.Data);
        }

        /// <summary>
        /// Updates product
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="product"></param>
        /// <returns>Updated product</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string productId,
            [FromBody] ProductModelDto product)
        {
            var result = await _productService.UpdateAsync(productId, product);
            return Ok(result.Data);
        }

        /// <summary>
        /// Deletes product
        /// </summary>
        /// <param name="productId"></param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string productId)
        {
            var result = await _productService.DeleteAsync(productId);
            return Ok(result.Data);
        }
    }
}