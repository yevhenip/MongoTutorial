using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Customers.Controllers.v1;
using Warehouse.Core.Common;
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
            _customerService.Setup(ms => ms.GetAllAsync())
                .ReturnsAsync(Result<List<CustomerDto>>.Success(customers));

            var result = await _customersController.GetAllAsync() as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(customers));
        }
        
        [Test]
        public async Task GetAsync_WhenCalled_ReturnsCustomer()
        {
            _customerService.Setup(us => us.GetAsync(_customer.Id))
                .ReturnsAsync(Result<CustomerDto>.Success(_customer));

            var result = await _customersController.GetAsync(_customer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_customer));
        }
        
        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsCustomer()
        {
            _customerService.Setup(us => us.CreateAsync(_customer))
                .ReturnsAsync(Result<CustomerDto>.Success(_customer));

            var result = await _customersController.CreateAsync(_customer) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_customer));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsCustomer()
        {
            _customerService.Setup(us => us.DeleteAsync(_customer.Id))
                .ReturnsAsync(Result<object>.Success());

            var result = await _customersController.DeleteAsync(_customer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }
    }
}