using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Auth.Controllers.v1;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Auth.Tests.Controllers
{
    [TestFixture]
    public class AuthControllerTests
    {
        private UserDto _user;
        private readonly Mock<IAuthService> _authService = new();

        private AuthController _authController;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _authController = new(_authService.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _user = new("a", "a", "a", "a", DateTime.UtcNow, "a", "a", new List<string> {"User"}, It.IsAny<string>());
        }

        [Test]
        public async Task Register_WhenCalled_ReturnsUserDto()
        {
            RegisterDto register = new("a", "a", "a", "a", "a", "a");
            _authService.Setup(us => us.RegisterAsync(register))
                .ReturnsAsync(Result<UserDto>.Success(_user));

            var result = await _authController.Register(register) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(_user));
        }

        [Test]
        public async Task Login_WhenCalled_ReturnsAuthenticatedUserDto()
        {
            LoginDto login = new("a", "a");
            UserAuthenticatedDto authenticated = new(_user, "a", "a");
            _authService.Setup(us => us.LoginAsync(login, It.IsAny<string>()))
                .ReturnsAsync(Result<UserAuthenticatedDto>.Success(authenticated));

            var result = await _authController.Login(login) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(authenticated));
        }

        [Test]
        public async Task RefreshToken_WhenCalled_ReturnsAuthenticatedUserDto()
        {
            TokenDto token = new("b");
            UserAuthenticatedDto authenticated = new(_user, "a", "a");
            ConfigureUser();
            _authService.Setup(us => us.RefreshTokenAsync(_user.Id, token, It.IsAny<string>()))
                .ReturnsAsync(Result<UserAuthenticatedDto>.Success(authenticated));

            var result = await _authController.RefreshToken(token) as OkObjectResult;

            Assert.That(result?.Value, Is.EqualTo(authenticated));
        }

        [Test]
        public async Task Logout_WhenCalled_ReturnsNull()
        {
            ConfigureUser();
            _authService.Setup(us => us.LogoutAsync(_user.Id))
                .ReturnsAsync(Result<object>.Success());

            await _authController.Logout();
        }

        private void ConfigureUser()
        {
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new("Id", "a")
            }));
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext {User = user}
            };
        }
    }
}