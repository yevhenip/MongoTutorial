using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Common;
using Warehouse.Api.Products.Commands;
using Warehouse.Api.Products.Controllers.v1;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Product;

namespace Warehouse.Api.Products.Tests.Controllers
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private ProductDto _product;
        private readonly Mock<IMediator> _mediator = new();

        private ProductsController _productsController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _productsController = new(_mediator.Object);
            _product = new("a", "a");
        }

        [Test]
        public async Task GetAsync_WhenCalled_ReturnsProduct()
        {
            _mediator.Setup(m => m.Send(new GetProductCommand(_product.Id), CancellationToken.None))
                .ReturnsAsync(Result<ProductDto>.Success(_product));

            var result = await _productsController.GetAsync(_product.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_product));
        }

        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsProduct()
        {
            ConfigureUser();
            ProductModelDto product = new("a", DateTime.Now, new List<string> {"a"}, "a");
            _mediator.Setup(m => m.Send(new CreateProductCommand(product, "a"), CancellationToken.None))
                .ReturnsAsync(Result<ProductDto>.Success(_product));

            var result = await _productsController.CreateAsync(product) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_product));
        }

        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsProduct()
        {
            ConfigureUser();
            ProductModelDto product = new("a", DateTime.Now, new List<string> {"a"}, "a");
            _mediator.Setup(m => m.Send(new UpdateProductCommand(_product.Id, product, "a"), CancellationToken.None))
                .ReturnsAsync(Result<ProductDto>.Success(_product));

            var result = await _productsController.UpdateAsync(_product.Id, product) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_product));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsProduct()
        {
            _mediator.Setup(m => m.Send(new DeleteProductCommand(_product.Id), CancellationToken.None))
                .ReturnsAsync(Result<object>.Success());

            var result = await _productsController.DeleteAsync(_product.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }

        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfProductDto()
        {
            PageDataDto<ProductDto> page = new(new List<ProductDto>(), 3);
            _mediator.Setup(m => m.Send(new GetProductsPageCommand(1, 1), CancellationToken.None))
                .ReturnsAsync(Result<PageDataDto<ProductDto>>.Success(page));

            var result = await _productsController.GetPageAsync(1, 1) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(page));
        }

        [Test]
        public async Task GetExportFileAsync_WhenCalled_ReturnsLogFile()
        {
            byte[] bytes = {1, 2, 3};
            _mediator.Setup(m => m.Send(new GetExportFileCommand(), CancellationToken.None))
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