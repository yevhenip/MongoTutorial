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
using Warehouse.Api.Manufacturers.Commands;
using Warehouse.Api.Manufacturers.Controllers.v1;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Manufacturer;

namespace Warehouse.Api.Manufacturers.Tests.Controllers
{
    [TestFixture]
    public class ManufacturersControllerTests
    {
        private ManufacturerDto _manufacturer;
        private readonly Mock<IMediator> _mediator = new();

        private ManufacturersController _manufacturersController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _manufacturersController = new(_mediator.Object);
            _manufacturer = new("a", "a", "a");
        }

        [Test]
        public async Task GetAsync_WhenCalled_ReturnsManufacturer()
        {
            _mediator.Setup(m => m.Send(new GetManufacturerCommand(_manufacturer.Id), CancellationToken.None))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.GetAsync(_manufacturer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }

        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsManufacturer()
        {
            ConfigureUser();
            ManufacturerModelDto manufacturerDto = new("a", "a");
            _mediator.Setup(m => m.Send(new CreateManufacturerCommand(manufacturerDto, "a"), CancellationToken.None))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.CreateAsync(manufacturerDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }

        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsManufacturer()
        {
            ConfigureUser();
            ManufacturerModelDto manufacturerDto = new("a", "a");
            _mediator.Setup(m => m.Send(new UpdateManufacturerCommand(_manufacturer.Id, manufacturerDto, "a"),
                    CancellationToken.None))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result =
                await _manufacturersController.UpdateAsync(_manufacturer.Id, manufacturerDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsManufacturer()
        {
            _mediator.Setup(m => m.Send(new DeleteManufacturerCommand(_manufacturer.Id), CancellationToken.None))
                .ReturnsAsync(Result<object>.Success());

            var result = await _manufacturersController.DeleteAsync(_manufacturer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }

        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfManufacturerDto()
        {
            PageDataDto<ManufacturerDto> page = new(new List<ManufacturerDto>(), 3);
            _mediator.Setup(m => m.Send(new GetManufacturersPageCommand(1, 1), CancellationToken.None))
                .ReturnsAsync(Result<PageDataDto<ManufacturerDto>>.Success(page));

            var result = await _manufacturersController.GetPageAsync(1, 1) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(page));
        }

        private void ConfigureUser()
        {
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new("UserName", "a"),
                new("Id", "a")
            }));
            _manufacturersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {User = user}
            };
        }
    }
}