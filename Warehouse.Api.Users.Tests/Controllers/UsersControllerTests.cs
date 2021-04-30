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
using Warehouse.Api.Users.Commands;
using Warehouse.Api.Users.Controllers.v1;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Users;

namespace Warehouse.Api.Users.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UserDto _user;
        private readonly Mock<IMediator> _mediator = new();

        private UsersController _usersController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _usersController = new(_mediator.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _user = new("a", "a", "a", "a", DateTime.UtcNow, "a", "a", new List<string> {"User"}, It.IsAny<string>());
        }

        [Test]
        public async Task GetAsync_WhenCalled_ReturnsUser()
        {
            _mediator.Setup(m => m.Send(new GetUserCommand(_user.Id), CancellationToken.None))
                .ReturnsAsync(Result<UserDto>.Success(_user));

            var result = await _usersController.GetAsync(_user.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_user));
        }

        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsUser()
        {
            ConfigureUser();
            UserModelDto userDto = new("a", "a", "a", "a");
            _mediator.Setup(m => m.Send(new UpdateUserCommand(_user.Id, userDto, "a"), CancellationToken.None))
                .ReturnsAsync(Result<UserDto>.Success(_user));

            var result = await _usersController.UpdateAsync(_user.Id, userDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_user));
        }

        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsUser()
        {
            _mediator.Setup(m => m.Send(new DeleteUserCommand(_user.Id), CancellationToken.None))
                .ReturnsAsync(Result<object>.Success());

            var result = await _usersController.DeleteAsync(_user.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }

        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfUserDto()
        {
            PageDataDto<UserDto> page = new(new List<UserDto>(), 3);
            _mediator.Setup(m => m.Send(new GetUsersPageCommand(1, 1), CancellationToken.None))
                .ReturnsAsync(Result<PageDataDto<UserDto>>.Success(page));

            var result = await _usersController.GetPageAsync(1, 1) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(page));
        }

        private void ConfigureUser()
        {
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new("UserName", "a"),
                new("Id", "a")
            }));
            _usersController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {User = user}
            };
        }
    }
}