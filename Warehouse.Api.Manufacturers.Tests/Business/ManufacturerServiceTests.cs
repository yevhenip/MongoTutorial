using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Manufacturers.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Manufacturer;
using Warehouse.Core.Interfaces.Messaging.Sender;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Manufacturers.Tests.Business
{
    [TestFixture]
    public class ManufacturerServiceTests
    {
        private ManufacturerService _manufacturerService;

        private readonly ManufacturerDto _manufacturer = new("a", "a", "a");
        private readonly Manufacturer _dbManufacturer = new() {Address = "a", Id = "a", Name = "a"};
        private readonly Mock<IManufacturerRepository> _manufacturerRepository = new();
        private readonly Mock<IOptions<CacheManufacturerSettings>> _options = new();
        private readonly Mock<IFileService> _fileService = new();
        private readonly Mock<IDistributedCache> _cache = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ISender> _sender = new();

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _options.Setup(opt => opt.Value).Returns(new CacheManufacturerSettings
                {AbsoluteExpiration = 1, SlidingExpiration = 1});
            _manufacturerService = new(_options.Object, _manufacturerRepository.Object, _cache.Object, _mapper.Object,
                _sender.Object, _fileService.Object);
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfManufacturers()
        {
            var manufacturers = ConfigureGetAll();

            var result = await _manufacturerService.GetAllAsync();

            Assert.That(result.Data, Is.EqualTo(manufacturers));
        }

        [Test]
        public async Task GetAsync_WhenCalledWithUnCashedManufacturer_ReturnsManufacturer()
        {
            ConfigureGet(_dbManufacturer, _dbManufacturer);

            var result = await _manufacturerService.GetAsync(_dbManufacturer.Id);

            Assert.That(result.Data, Is.EqualTo(_manufacturer));
        }

        [Test]
        public async Task GetAsync_WhenCalledWithFiledManufacturer_ReturnsManufacturer()
        {
            ConfigureGet(null, _dbManufacturer);

            var result = await _manufacturerService.GetAsync(_dbManufacturer.Id);

            Assert.That(result.Data, Is.EqualTo(_manufacturer));
        }

        [Test]
        public void GetAsync_WhenCalledWithUndefinedManufacturer_ThrowsResultOfManufacturerException()
        {
            ConfigureGet(null, null);

            Assert.ThrowsAsync<Result<Manufacturer>>(
                async () => await _manufacturerService.GetAsync(_dbManufacturer.Id));
        }

        [Test]
        public async Task CreateAsync_WhenCalledWithUniqueData_ReturnsManufacturer()
        {
            var manufacturer = ConfigureCreate();

            var result = await _manufacturerService.CreateAsync(manufacturer);

            Assert.That(result.Data, Is.EqualTo(_manufacturer));
        }
        
        [Test]
        public async Task UpdateAsync_WhenWithCalledUnCashedManufacturer_ReturnsManufacturer()
        {
            var manufacturer = ConfigureUpdate(_dbManufacturer, null);

            var result = await _manufacturerService.UpdateAsync(_dbManufacturer.Id, manufacturer);

            Assert.That(result.Data, Is.EqualTo(_manufacturer));
        }

        [Test]
        public async Task UpdateAsync_WhenWithCalledUnFiledManufacturer_ReturnsManufacturer()
        {
            var manufacturer = ConfigureUpdate(null, _dbManufacturer);

            var result = await _manufacturerService.UpdateAsync(_dbManufacturer.Id, manufacturer);

            Assert.That(result.Data, Is.EqualTo(_manufacturer));
        }

        [Test]
        public void UpdateAsync_WhenWithCalledUndefinedManufacturer_ThrowsResultOfManufacturerException()
        {
            var manufacturer = ConfigureUpdate(null, null);

            Assert.ThrowsAsync<Result<Manufacturer>>(async () => await _manufacturerService.UpdateAsync(_dbManufacturer.Id, manufacturer));
        }

        [Test]
        public void DeleteAsync_WhenCalledWithUnCachedManufacturerAndUnExistingId_ThrowsResultOfManufacturerException()
        {
            ConfigureDelete(null);

            Assert.ThrowsAsync<Result<Manufacturer>>(async () =>
                await _manufacturerService.DeleteAsync(_dbManufacturer.Id));
        }

        [Test]
        public async Task DeleteAsync_WhenCalledWithUnCachedManufacturerAndExistingId_ReturnsNull()
        {
            ConfigureDelete(_dbManufacturer);
            var result = await _manufacturerService.DeleteAsync(_dbManufacturer.Id);

            Assert.That(result.Data, Is.EqualTo(null));
        }

        [Test]
        public async Task GetRangeAsync_WhenCalled_ReturnsListOfManufacturers()
        {
            var (manufacturers, manufacturerIds) = ConfigureGetRange();

            var result = await _manufacturerService.GetRangeAsync(manufacturerIds);

            Assert.That(result.Data, Is.EqualTo(manufacturers));
        }

        private List<ManufacturerDto> ConfigureGetAll()
        {
            List<ManufacturerDto> manufacturers = new() {_manufacturer};
            List<Manufacturer> manufacturersFromDb = new() {_dbManufacturer};
            _manufacturerRepository.Setup(ur => ur.GetAllAsync()).ReturnsAsync(manufacturersFromDb);
            _mapper.Setup(m => m.Map<List<ManufacturerDto>>(manufacturersFromDb)).Returns(manufacturers);
            return manufacturers;
        }

        private void ConfigureGet(Manufacturer dbManufacturer, Manufacturer fileManufacturer)
        {
            _manufacturerRepository.Setup(ur => ur.GetAsync(_dbManufacturer.Id)).ReturnsAsync(dbManufacturer);
            _fileService.Setup(ur => ur.ReadFromFileAsync<Manufacturer>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileManufacturer);
            _mapper.Setup(m => m.Map<ManufacturerDto>(_dbManufacturer)).Returns(_manufacturer);
        }

        private ManufacturerModelDto ConfigureCreate()
        {
            ManufacturerModelDto manufacturer = new("a", "a");
            _mapper.Setup(m => m.Map<ManufacturerDto>(manufacturer)).Returns(_manufacturer);
            _mapper.Setup(m => m.Map<Manufacturer>(_manufacturer)).Returns(_dbManufacturer);
            return manufacturer;
        }

        private ManufacturerModelDto ConfigureUpdate(Manufacturer dbManufacturer, Manufacturer fileManufacturer)
        {
            ManufacturerModelDto manufacturer = new("a", "a");
            _manufacturerRepository.Setup(ur => ur.GetAsync(_dbManufacturer.Id)).ReturnsAsync(dbManufacturer);
            _fileService.Setup(ur => ur.ReadFromFileAsync<Manufacturer>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileManufacturer);
            _mapper.Setup(m => m.Map<ManufacturerDto>(manufacturer)).Returns(_manufacturer);

            return manufacturer;
        }
        
        private void ConfigureDelete(Manufacturer manufacturer)
        {
            _manufacturerRepository.Setup(ur => ur.GetAsync(_dbManufacturer.Id)).ReturnsAsync(manufacturer);
        }

        private (List<ManufacturerDto>, IEnumerable<string>) ConfigureGetRange()
        {
            List<ManufacturerDto> manufacturers = new() {_manufacturer};
            List<Manufacturer> manufacturersFromDb = new() {_dbManufacturer};
            var manufacturerIds = manufacturers.Select(m => m.Id);
            _manufacturerRepository.Setup(ur => ur.GetRangeAsync(manufacturerIds))
                .ReturnsAsync(manufacturersFromDb);
            _mapper.Setup(m => m.Map<List<ManufacturerDto>>(manufacturersFromDb)).Returns(manufacturers);
            return (manufacturers, manufacturerIds);
        }
    }
}