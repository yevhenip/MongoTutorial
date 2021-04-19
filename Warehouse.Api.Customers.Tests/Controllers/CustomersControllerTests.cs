using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Customers.Controllers.v1;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Customers.Tests.Controllers
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private CustomerDto _customer;
        private readonly Mock<ICustomerService> _customerService = new();
        
        private CustomersController _customersController;
        
        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _customersController = new(_customerService.Object);
            _customer = new("a", "a", "a", "a");
        }
        
        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfCustomers()
        {
            List<CustomerDto> customers = new(){_customer};
            _customerService.Setup(cs => cs.GetAllAsync())
                .ReturnsAsync(Result<List<CustomerDto>>.Success(customers));

            var result = await _customersController.GetAllAsync() as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(customers));
        }
        
        [Test]
        public async Task GetAsync_WhenCalled_ReturnsCustomer()
        {
            _customerService.Setup(cs => cs.GetAsync(_customer.Id))
                .ReturnsAsync(Result<CustomerDto>.Success(_customer));

            var result = await _customersController.GetAsync(_customer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_customer));
        }
        
        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsCustomer()
        {
            ConfigureUser();
            _customerService.Setup(cs => cs.CreateAsync(_customer, "a"))
                .ReturnsAsync(Result<CustomerDto>.Success(_customer));

            var result = await _customersController.CreateAsync(_customer) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_customer));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsCustomer()
        {
            _customerService.Setup(cs => cs.DeleteAsync(_customer.Id))
                .ReturnsAsync(Result<object>.Success());

            var result = await _customersController.DeleteAsync(_customer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }
        
        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfCustomerDto()
        {
            PageDataDto<CustomerDto> page = new(new List<CustomerDto>(), 3);
            _customerService.Setup(cs => cs.GetPageAsync(1, 1))
                .ReturnsAsync(Result<PageDataDto<CustomerDto>>.Success(page));

            var result = await _customersController.GetPageAsync(1, 1) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(page));
        }
        
        private void ConfigureUser()
        {
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new("UserName", "a"),
                new("Id", "a")
            }));
            _customersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {User = user}
            };
        }
    }
}