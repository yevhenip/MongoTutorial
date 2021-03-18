using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Users.Controllers.v1;
using Warehouse.Core.Common;
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
            UserModelDto userDto = new("a", "a", "a", "a");
            _userService.Setup(us => us.UpdateAsync(_user.Id, userDto))
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
    }
}