using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Users.Controllers.v1;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users.Tests.Controllers
{
    [TestFixture]
    public class UsersControllerTests
    {
        private UserDto _user;
        private readonly Mock<IUserService> _userService = new();

        private UsersController _usersController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _usersController = new(_userService.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _user = new("a", "a", "a", "a", DateTime.UtcNow, "a", "a", new List<string> {"User"}, It.IsAny<string>());
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfUsers()
        {
            List<UserDto> users = new(){_user};
            _userService.Setup(us => us.GetAllAsync())
                .ReturnsAsync(Result<List<UserDto>>.Success(users));

            var result = await _usersController.GetAllAsync() as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(users));
        }
        
        [Test]
        public async Task GetAsync_WhenCalled_ReturnsUser()
        {
            _userService.Setup(us => us.GetAsync(_user.Id))
                .ReturnsAsync(Result<UserDto>.Success(_user));

            var result = await _usersController.GetAsync(_user.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_user));
        }
        
        [Test]
        public async Task UpdateAsync_WhenCalled_ReturnsUser()
        {
            ConfigureUser();
            UserModelDto userDto = new("a", "a", "a", "a");
            _userService.Setup(us => us.UpdateAsync(_user.Id, userDto, "a"))
                .ReturnsAsync(Result<UserDto>.Success(_user));

            var result = await _usersController.UpdateAsync(_user.Id, userDto) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_user));
        }
        
        [Test]
        public async Task DeleteAsync_WhenCalled_ReturnsUser()
        {
            _userService.Setup(us => us.DeleteAsync(_user.Id))
                .ReturnsAsync(Result<object>.Success());

            var result = await _usersController.DeleteAsync(_user.Id) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(null));
        }
        
        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDataDtoOfUserDto()
        {
            PageDataDto<UserDto> page = new(new List<UserDto>(), 3);
            _userService.Setup(us => us.GetPageAsync(1, 1))
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