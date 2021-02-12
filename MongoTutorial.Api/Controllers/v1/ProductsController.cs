using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoTutorial.Api.Extensions;
using MongoTutorial.Api.Models.Product;
using MongoTutorial.Core.Dtos;
using MongoTutorial.Core.Interfaces.Services;

namespace MongoTutorial.Api.Controllers.v1
{
    public class ProductsController : ApiControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync()
        {
            var result = await _productService.GetProductsAsync();
            return result.Succeeded
                ? Ok(result.ToApiResponse())
                : BadRequest(result.ToApiError());
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] string productId)
        {
            var result = await _productService.GetProductByIdAsync(productId);
            return result.Succeeded
                ? Ok(result.ToApiResponse())
                : BadRequest(result.ToApiError());
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductModel product)
        {
            var productToDb = _mapper.Map<ProductDto>(product);
            var result = await _productService.CreateProductAsync(productToDb);
            return result.Succeeded
                ? Ok(result.ToApiResponse())
                : BadRequest(result.ToApiError());
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync([FromRoute] string productId,
            [FromBody] ProductModel product)
        {
            var productToDb = _mapper.Map<ProductDto>(product) with {Id = productId};
            var result = await _productService.UpdateProductAsync(productToDb);
            return result.Succeeded
                ? Ok(result.ToApiResponse())
                : BadRequest(result.ToApiError());
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] string productId)
        {
            var result = await _productService.DeleteProductAsync(productId);
            return result.Succeeded
                ? Ok(result.ToApiResponse())
                : BadRequest(result.ToApiError());
        }
    }
}