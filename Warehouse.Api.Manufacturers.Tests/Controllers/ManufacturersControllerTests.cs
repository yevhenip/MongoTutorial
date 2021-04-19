using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Manufacturers.Controllers.v1;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Manufacturers.Tests.Controllers
{
    [TestFixture]
    public class ManufacturersControllerTests
    {
        private ManufacturerDto _manufacturer;
        private readonly Mock<IManufacturerService> _manufacturerService = new();
        
        private ManufacturersController _manufacturersController;
        
        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _manufacturersController = new(_manufacturerService.Object);
            _manufacturer = new("a", "a", "a");
        }
        
        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfManufacturers()
        {
            List<ManufacturerDto> manufacturers = new(){_manufacturer};
            _manufacturerService.Setup(ms => ms.GetAllAsync())
                .ReturnsAsync(Result<List<ManufacturerDto>>.Success(manufacturers));

            var result = await _manufacturersController.GetAllAsync() as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(manufacturers));
        }
        
        [Test]
        public async Task GetAsync_WhenCalled_ReturnsManufacturer()
        {
            _manufacturerService.Setup(ms => ms.GetAsync(_manufacturer.Id))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.GetAsync(_manufacturer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }
        
        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsManufacturer()
        {
            ConfigureUser();
            ManufacturerModelDto manufacturerDto = new("a", "a");
            _manufacturerService.Setup(ms => ms.CreateAsync(manufacturerDto, "a"))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.CreateAsync(manufacturerDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }
        
        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsManufacturer()
        {
            ConfigureUser();
            ManufacturerModelDto manufacturerDto = new("a", "a");
            _manufacturerService.Setup(ms => ms.UpdateAsync(_manufacturer.Id, manufacturerDto, "a"))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.UpdateAsync(_manufacturer.Id, manufacturerDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }
        
        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsManufacturer()
        {
            _manufacturerService.Setup(ms => ms.DeleteAsync(_manufacturer.Id))
                .ReturnsAsync(Result<object>.Success());

            var result = await _manufacturersController.DeleteAsync(_manufacturer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }
        
        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfManufacturerDto()
        {
            PageDataDto<ManufacturerDto> page = new(new List<ManufacturerDto>(), 3);
            _manufacturerService.Setup(ms => ms.GetPageAsync(1, 1))
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