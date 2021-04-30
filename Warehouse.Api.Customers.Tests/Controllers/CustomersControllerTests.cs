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
using Warehouse.Api.Customers.Commands;
using Warehouse.Api.Customers.Controllers.v1;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Customer;

namespace Warehouse.Api.Customers.Tests.Controllers
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private CustomerDto _customer;
        private readonly Mock<IMediator> _mediator = new();

        private CustomersController _customersController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _customersController = new(_mediator.Object);
            _customer = new("a", "a", "a", "a");
        }
        
        [Test]
        public async Task GetAsync_WhenCalled_ReturnsCustomer()
        {
            _mediator.Setup(m => m.Send(new GetCustomerCommand(_customer.Id), CancellationToken.None))
                .ReturnsAsync(Result<CustomerDto>.Success(_customer));

            var result = await _customersController.GetAsync(_customer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_customer));
        }

        [Test]
        public async Task CreateAsync_WhenCalled_ReturnsCustomer()
        {
            ConfigureUser();
            _mediator.Setup(m => m.Send(new CreateCustomerCommand(_customer, "a"), CancellationToken.None))
                .ReturnsAsync(Result<CustomerDto>.Success(_customer));

            var result = await _customersController.CreateAsync(_customer) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_customer));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsCustomer()
        {
            _mediator.Setup(m => m.Send(new DeleteCustomerCommand(_customer.Id), CancellationToken.None))
                .ReturnsAsync(Result<object>.Success());

            var result = await _customersController.DeleteAsync(_customer.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }

        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfCustomerDto()
        {
            PageDataDto<CustomerDto> page = new(new List<CustomerDto>(), 3);
            _mediator.Setup(m => m.Send(new GetCustomerPageCommand(1, 1), CancellationToken.None))
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