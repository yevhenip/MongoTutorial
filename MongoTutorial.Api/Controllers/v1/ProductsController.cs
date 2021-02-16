using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
            var result = await _productService.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductByIdAsync([FromRoute] string productId)
        {
            var result = await _productService.GetAsync(productId);
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync([FromBody] ProductModel product)
        {
            var productToDb = _mapper.Map<ProductDto>(product);
            var result = await _productService.CreateAsync(productToDb, product.ManufacturerIds.ToList());
            return Ok(result.Data);
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProductAsync([FromRoute] string productId,
            [FromBody] ProductModel product)
        {
            var productToDb = _mapper.Map<ProductDto>(product) with {Id = productId};
            var result = await _productService.UpdateAsync(productToDb, product.ManufacturerIds.ToList());
            return Ok(result.Data);
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] string productId)
        {
            var result = await _productService.DeleteAsync(productId);
            return Ok(result.Data);
        }
    }
}