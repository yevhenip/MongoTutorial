using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Common;
using Warehouse.Core.Common;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Tests.Common
{
    [TestFixture]
    public class ValidateSessionIdTests
    {
        private readonly User _user = new()
        {
            Id = "a", Email = "a", Phone = "a", FullName = "a", RegistrationDateTime = DateTime.UtcNow,
            PasswordHash = "a", UserName = "a", Roles = new List<string> {"User"}, SessionId = "a"
        };

        private readonly Mock<IUserRepository> _userRepository = new();

        private ValidateTokenSessionId _validateTokenSessionId;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _validateTokenSessionId = new(_userRepository.Object);
        }

        [Test]
        public void TokenValidated_WhenCalledWithWrongSessionId_ThrowsResultOfObjectException()
        {
            var context = ConfigureTests("b");

            Assert.ThrowsAsync<Result<object>>(async () => await _validateTokenSessionId.TokenValidated(context));
        }

        [Test]
        public async Task TokenValidated_WhenCalledWithRightSessionId_ReturnsNull()
        {
            var context = ConfigureTests("a");

            await _validateTokenSessionId.TokenValidated(context);
        }

        private TokenValidatedContext ConfigureTests(string session)
        {
            ClaimsPrincipal user = new(new ClaimsIdentity(new Claim[]
            {
                new("Id", "a"),
                new("SessionId", session)
            }));
            _userRepository.Setup(ur => ur.GetAsync(u =>u.Id == _user.Id)).ReturnsAsync(_user);
            return new(
                new DefaultHttpContext(),
                new AuthenticationScheme("Test", "Test", typeof(IAuthenticationHandler)),
                new JwtBearerOptions()) {Principal = user};
        }
    }
}