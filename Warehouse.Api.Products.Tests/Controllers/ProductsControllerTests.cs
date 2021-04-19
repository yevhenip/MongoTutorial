using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Products.Controllers.v1;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Products.Tests.Controllers
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private ProductDto _product;
        private readonly Mock<IProductService> _productService = new();

        private ProductsController _productsController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _productsController = new(_productService.Object);
            _product = new("a", "a");
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfProducts()
        {
            List<ProductDto> products = new() {_product};
            _productService.Setup(ps => ps.GetAllAsync())
                .ReturnsAsync(Result<List<ProductDto>>.Success(products));

            var result = await _productsController.GetAllAsync() as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(products));
        }

        [Test]
        public async Task GetAsync_WhenCalled_ReturnsProduct()
        {
            _productService.Setup(ps => ps.GetAsync(_product.Id))
                .ReturnsAsync(Result<ProductDto>.Success(_product));

            var result = await _productsController.GetAsync(_product.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_product));
        }

        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsProduct()
        {
            ConfigureUser();
            ProductModelDto product = new("a", DateTime.Now, new List<string> {"a"}, "a");
            _productService.Setup(ps => ps.CreateAsync(product, "a"))
                .ReturnsAsync(Result<ProductDto>.Success(_product));

            var result = await _productsController.CreateAsync(product) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_product));
        }

        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsProduct()
        {
            ConfigureUser();
            ProductModelDto product = new("a", DateTime.Now, new List<string> {"a"}, "a");
            _productService.Setup(ps => ps.UpdateAsync(_product.Id, product, "a"))
                .ReturnsAsync(Result<ProductDto>.Success(_product));

            var result = await _productsController.UpdateAsync(_product.Id, product) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_product));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsProduct()
        {
            _productService.Setup(ps => ps.DeleteAsync(_product.Id))
                .ReturnsAsync(Result<object>.Success());

            var result = await _productsController.DeleteAsync(_product.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }

        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfProductDto()
        {
            PageDataDto<ProductDto> page = new(new List<ProductDto>(), 3);
            _productService.Setup(ps => ps.GetPageAsync(1, 1))
                .ReturnsAsync(Result<PageDataDto<ProductDto>>.Success(page));

            var result = await _productsController.GetPageAsync(1, 1) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(page));
        }
        
        [Test]
        public async Task GetExportFileAsync_WhenCalled_ReturnsLogFile()
        {
            byte[] bytes = {1, 2, 3};
            _productService.Setup(ps => ps.GetExportFileAsync())
                .ReturnsAsync(Result<byte[]>.Success(bytes));

            var result = await _productsController.GetExportFileAsync() as FileContentResult;

            Assert.That(result?.FileContents, Is.EqualTo(bytes));
        }

        private void ConfigureUser()
        {
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new("UserName", "a"),
                new("Id", "a")
            }));
            _productsController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {User = user}
            };
        }
    }
}