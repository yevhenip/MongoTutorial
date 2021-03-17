using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Manufacturers.Controllers.v1;
using Warehouse.Core.Common;
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
            _manufacturerService.Setup(us => us.GetAsync(_manufacturer.Id))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.GetAsync(_manufacturer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }
        
        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsManufacturer()
        {
            ManufacturerModelDto manufacturerDto = new("a", "a");
            _manufacturerService.Setup(us => us.CreateAsync(manufacturerDto))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.CreateAsync(manufacturerDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }
        
        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsManufacturer()
        {
            ManufacturerModelDto manufacturerDto = new("a", "a");
            _manufacturerService.Setup(us => us.UpdateAsync(_manufacturer.Id, manufacturerDto))
                .ReturnsAsync(Result<ManufacturerDto>.Success(_manufacturer));

            var result = await _manufacturersController.UpdateAsync(_manufacturer.Id, manufacturerDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_manufacturer));
        }
        
        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsManufacturer()
        {
            _manufacturerService.Setup(us => us.DeleteAsync(_manufacturer.Id))
                .ReturnsAsync(Result<object>.Success());

            var result = await _manufacturersController.DeleteAsync(_manufacturer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }
    }
}