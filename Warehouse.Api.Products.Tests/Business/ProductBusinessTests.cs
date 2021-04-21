using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Products.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.DTO.Product;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Products.Tests.Business
{
    [TestFixture]
    public class ProductServiceTests
    {
        private ProductService _productService;

        private readonly ProductDto _product = new("a", "a", DateTime.Now, new List<Manufacturer>(),
            new CustomerDto("a", "a", "a", "a"));

        private readonly Product _dbProduct = new()
            {Customer = new Customer {Id = "a"}, Id = "a", Name = "a", DateOfReceipt = DateTime.Now};

        private readonly Mock<IProductRepository> _productRepository = new();
        private readonly Mock<ICustomerRepository> _customerRepository = new();
        private readonly Mock<IManufacturerRepository> _manufacturerRepository = new();
        private readonly Mock<IOptions<CacheProductSettings>> _productOptions = new();
        private readonly Mock<IOptions<CacheManufacturerSettings>> _manufacturerOptions = new();
        private readonly Mock<IFileService> _fileService = new();
        private readonly Mock<IDistributedCache> _cache = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<IBus> _bus = new();

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _productOptions.Setup(opt => opt.Value).Returns(new CacheProductSettings
                {AbsoluteExpiration = 1, SlidingExpiration = 1});
            _manufacturerOptions.Setup(opt => opt.Value).Returns(new CacheManufacturerSettings
                {AbsoluteExpiration = 1, SlidingExpiration = 1});
            _productService = new(_productRepository.Object, _cache.Object, _productOptions.Object,
                _manufacturerOptions.Object, _customerRepository.Object, _mapper.Object, _manufacturerRepository.Object,
                _fileService.Object, _bus.Object);
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfProducts()
        {
            var products = ConfigureGetAll();

            var result = await _productService.GetAllAsync();

            Assert.That(result.Data, Is.EqualTo(products));
        }

        [Test]
        public async Task GetAsync_WhenCalledWithUnCashedProduct_ReturnsProduct()
        {
            ConfigureGet(_dbProduct, _dbProduct);

            var result = await _productService.GetAsync(_dbProduct.Id);

            Assert.That(result.Data, Is.EqualTo(_product));
        }

        [Test]
        public async Task GetAsync_WhenCalledWithFiledProduct_ReturnsProduct()
        {
            ConfigureGet(null, _dbProduct);

            var result = await _productService.GetAsync(_dbProduct.Id);

            Assert.That(result.Data, Is.EqualTo(_product));
        }

        [Test]
        public void GetAsync_WhenCalledWithUndefinedProduct_ThrowsResultOfProductException()
        {
            ConfigureGet(null, null);

            Assert.ThrowsAsync<Result<Product>>(
                async () => await _productService.GetAsync(_dbProduct.Id));
        }

        [Test]
        public async Task CreateAsync_WhenCalledWithUniqueData_ReturnsProduct()
        {
            var product = ConfigureCreate();

            var result = await _productService.CreateAsync(product, "a");

            Assert.That(result.Data, Is.EqualTo(_product));
        }

        [Test]
        public async Task UpdateAsync_WhenWithCalledUnCashedProduct_ReturnsProduct()
        {
            var product = ConfigureUpdate(_dbProduct, null);

            var result = await _productService.UpdateAsync(_dbProduct.Id, product, "a");

            Assert.That(result.Data, Is.EqualTo(_product));
        }

        [Test]
        public async Task UpdateAsync_WhenWithCalledUnFiledProduct_ReturnsProduct()
        {
            var product = ConfigureUpdate(null, _dbProduct);

            var result = await _productService.UpdateAsync(_dbProduct.Id, product, "a");

            Assert.That(result.Data, Is.EqualTo(_product));
        }

        [Test]
        public void UpdateAsync_WhenWithCalledUndefinedProduct_ThrowsResultOfProductException()
        {
            var product = ConfigureUpdate(null, null);

            Assert.ThrowsAsync<Result<Product>>(async () =>
                await _productService.UpdateAsync(_dbProduct.Id, product, "a"));
        }

        [Test]
        public void DeleteAsync_WhenCalledWithUnCachedProductAndUnExistingId_ThrowsResultOfProductException()
        {
            ConfigureDelete(null);

            Assert.ThrowsAsync<Result<Product>>(async () =>
                await _productService.DeleteAsync(_dbProduct.Id));
        }

        [Test]
        public async Task DeleteAsync_WhenCalledWithUnCachedProductAndExistingId_ReturnsNull()
        {
            ConfigureDelete(_dbProduct);

            var result = await _productService.DeleteAsync(_dbProduct.Id);

            Assert.That(result.Data, Is.EqualTo(null));
        }

        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDateDtoOfProductDto()
        {
            var expected = ConfigureGetPage();

            var result = await _productService.GetPageAsync(1, 1);

            Assert.That(result.Data, Is.EqualTo(expected));
        }

        [Test]
        public async Task GetExportFileAsync_WhenCalled_ReturnsByteArray()
        {
            ConfigureGetFile();

            var result = await _productService.GetExportFileAsync();

            Assert.That(result.Data, Is.EqualTo(Array.Empty<byte>()));
        }

        private List<ProductDto> ConfigureGetAll()
        {
            List<ProductDto> products = new() {_product};
            List<Product> productsFromDb = new() {_dbProduct};
            _productRepository.Setup(pr => pr.GetRangeAsync(_ => true)).ReturnsAsync(productsFromDb);
            _mapper.Setup(m => m.Map<List<ProductDto>>(productsFromDb)).Returns(products);
            return products;
        }

        private void ConfigureGet(Product dbProduct, Product fileProduct)
        {
            _productRepository.Setup(pr => pr.GetAsync(p => p.Id == _dbProduct.Id)).ReturnsAsync(dbProduct);
            _fileService.Setup(fs => fs.ReadFromFileAsync<Product>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileProduct);
            _mapper.Setup(m => m.Map<ProductDto>(_dbProduct)).Returns(_product);
        }

        private ProductModelDto ConfigureCreate()
        {
            ProductModelDto product = new("a", DateTime.Now, new List<string> {"a"}, "a");
            _mapper.Setup(m => m.Map<ProductDto>(product)).Returns(_product);
            _mapper.Setup(m => m.Map<Product>(_product)).Returns(_dbProduct);
            _manufacturerRepository.Setup(ms => ms.GetRangeAsync(m => It.IsAny<IEnumerable<string>>().Contains(m.Id)))
                .ReturnsAsync(new List<Manufacturer> {new()});
            _customerRepository.Setup(cr => cr.GetAsync(c => c.Id == _dbProduct.Customer.Id))
                .ReturnsAsync(_dbProduct.Customer);
            _mapper.Setup(m => m.Map<Product>(product)).Returns(_dbProduct);
            _mapper.Setup(m => m.Map<ProductDto>(_dbProduct)).Returns(_product);
            _mapper.Setup(m => m.Map<Customer>(_dbProduct.Customer)).Returns(_dbProduct.Customer);
            return product;
        }

        private ProductModelDto ConfigureUpdate(Product dbProduct, Product fileProduct)
        {
            ProductModelDto product = new("a", DateTime.Now, new List<string> {"a"}, "a");
            _productRepository.Setup(pr => pr.GetAsync(p => p.Id == _dbProduct.Id)).ReturnsAsync(dbProduct);
            _fileService.Setup(fs => fs.ReadFromFileAsync<Product>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileProduct);
            _manufacturerRepository.Setup(mr => mr.GetRangeAsync(m => It.IsAny<IEnumerable<string>>().Contains(m.Id)))
                .ReturnsAsync(new List<Manufacturer> {new()});
            _customerRepository.Setup(cr => cr.GetAsync(c => c.Id == _dbProduct.Customer.Id))
                .ReturnsAsync(_dbProduct.Customer);
            _mapper.Setup(m => m.Map<Product>(product)).Returns(_dbProduct);
            _mapper.Setup(m => m.Map<ProductDto>(_dbProduct)).Returns(_product);
            _mapper.Setup(m => m.Map<Customer>(_dbProduct.Customer)).Returns(_dbProduct.Customer);

            return product;
        }

        private void ConfigureDelete(Product product)
        {
            _productRepository.Setup(pr => pr.GetAsync(p => p.Id == _dbProduct.Id)).ReturnsAsync(product);
        }

        private PageDataDto<ProductDto> ConfigureGetPage()
        {
            List<ProductDto> productDtos = new();
            List<Product> products = new();
            PageDataDto<ProductDto> expected = new(productDtos, 1);
            _productRepository.Setup(pr => pr.GetPageAsync(1, 1))
                .ReturnsAsync(products);
            _productRepository.Setup(pr => pr.GetCountAsync(_ => true))
                .ReturnsAsync(1);
            _mapper.Setup(m => m.Map<List<ProductDto>>(products)).Returns(productDtos);
            return expected;
        }

        private void ConfigureGetFile()
        {
            ConfigureGetAll();
            _mapper.Setup(m => m.Map<List<ExportProduct>>(new List<Product> {_dbProduct}))
                .Returns(new List<ExportProduct>());
        }
    }
}