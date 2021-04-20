using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Auth.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings;
using Warehouse.Domain;

namespace Warehouse.Api.Auth.Tests.Business
{
    [TestFixture]
    public class AuthServiceTests
    {
        private UserDto _user;

        private readonly User _userFromDb = new()
        {
            Id = "a", Email = "a", Phone = "a", FullName = "a", RegistrationDateTime = DateTime.UtcNow,
            PasswordHash = "a", UserName = "a", Roles = new List<string> {"User"}, SessionId = "a"
        };

        private readonly Mock<IRefreshTokenRepository> _tokenRepository = new();
        private readonly Mock<IOptions<JwtTokenConfiguration>> _options = new();
        private readonly Mock<IPasswordHasher<UserDto>> _hasher = new();
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IMapper> _mapper = new();

        private IAuthService _authService;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            Mock<IDistributedCache> cache = new();
            Mock<IBus> bus = new();
            _options.Setup(opt => opt.Value).Returns(new JwtTokenConfiguration
            {
                Audience = "Test", Issuer = "Test", Secret = ":-)TestTestTestTestTestTestTestTest(-:",
                AccessTokenExpirationMinutes = 10, RefreshTokenExpirationMinutes = 10
            });

            _authService = new AuthService(_options.Object, _tokenRepository.Object, cache.Object, _mapper.Object,
                _userRepository.Object, bus.Object, _hasher.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _user = new("a", "a", "a", "a", DateTime.UtcNow, "a", "a", new List<string> {"User"}, "a");
        }

        [Test]
        public async Task RegisterAsync_WhenCalled_ReturnedResultOfUserDto()
        {
            RegisterDto register = new("a", "a", "a", "a", "a", "a");
            var user = _user with {SessionId = "a"};
            _mapper.Setup(m => m.Map<UserDto>(register)).Returns(user);
            _mapper.Setup(m => m.Map<User>(user)).Returns(_userFromDb);
            _hasher.Setup(h => h.HashPassword(user, register.Password)).Returns("a");

            var result = await _authService.RegisterAsync(register);

            Assert.That(result.Data.User.ToString(), Is.EqualTo(user.ToString()));
        }

        [Test]
        public async Task LoginAsync_WhenCalled_ReturnedAuthenticatedDto()
        {
            var login = ConfigureLoginTests(PasswordVerificationResult.Success, _userFromDb);

            var result = await _authService.LoginAsync(login, _user.SessionId);

            Assertion(result);
        }

        [Test]
        public void LoginAsync_WhenCalledWithWrongPassword_ThrowsResultOfUserAuthenticatedDtoException()
        {
            var login = ConfigureLoginTests(PasswordVerificationResult.Failed, _userFromDb);

            Assert.ThrowsAsync<Result<UserAuthenticatedDto>>(async () =>
                await _authService.LoginAsync(login, _user.SessionId));
        }

        [Test]
        public void LoginAsync_WhenCalledWithWrongUserName_ThrowsResultOfUserException()
        {
            var login = ConfigureLoginTests(PasswordVerificationResult.Failed, null);

            Assert.ThrowsAsync<Result<User>>(async () =>
                await _authService.LoginAsync(login, _user.SessionId));
        }

        [Test]
        public async Task RefreshTokenAsync_WhenCalled_ReturnedAuthenticatedDto()
        {
            var token = ConfigureRefreshTokenTests(2222);

            var result = await _authService.RefreshTokenAsync(_user.Id, token, _user.SessionId);

            Assertion(result);
        }

        [Test]
        public void RefreshTokenAsync_WhenCalledWithExpiredToken_ThrowsResultOfRefreshTokenException()
        {
            var token = ConfigureRefreshTokenTests(1);

            Assert.ThrowsAsync<Result<RefreshToken>>(async () =>
                await _authService.RefreshTokenAsync(_user.Id, token, _user.SessionId));
        }

        [Test]
        public async Task LogoutAsync_WhenCalled_ReturnsNull()
        {
            var id = ConfigureLogoutTests(_userFromDb);

            var result = await _authService.LogoutAsync(id);

            Assert.That(result.Data, Is.EqualTo(null));
        }

        [Test]
        public void LogoutAsync_WhenCalledWithWrongId_ThrowsResultOfUserException()
        {
            var id = ConfigureLogoutTests(null);

            Assert.ThrowsAsync<Result<User>>(async () =>
                await _authService.LogoutAsync(id));
        }

        private LoginDto ConfigureLoginTests(PasswordVerificationResult result, User user)
        {
            LoginDto login = new("a", "a");

            _userRepository.Setup(ur => ur.GetByUserNameAsync(_user.UserName)).ReturnsAsync(user);
            _mapper.Setup(m => m.Map<UserDto>(_userFromDb)).Returns(_user);
            _hasher.Setup(h => h.VerifyHashedPassword(_user, _user.PasswordHash, login.Password))
                .Returns(result);

            return login;
        }

        private TokenDto ConfigureRefreshTokenTests(int year)
        {
            TokenDto token = new("a");

            _userRepository.Setup(ur => ur.GetAsync(_user.UserName)).ReturnsAsync(_userFromDb);
            _tokenRepository.Setup(tr => tr.GetAsync(_user.Id, token.Name)).ReturnsAsync(new RefreshToken
            {
                Token = token.Name,
                DateCreated = new DateTime(1, 1, 1),
                User = _userFromDb,
                Id = "a",
                DateExpires = new DateTime(year, 1, 1)
            });

            return token;
        }

        private string ConfigureLogoutTests(User user)
        {
            const string id = "a";
            _userRepository.Setup(ur => ur.GetAsync(id)).ReturnsAsync(user);
            return id;
        }

        private void Assertion(Result<UserAuthenticatedDto> result)
        {
            JwtSecurityTokenHandler handler = new();
            var jwt = _options.Object.Value;
            var validTo = DateTime.UtcNow.AddMinutes(jwt.AccessTokenExpirationMinutes);
            var tokens = handler.ReadToken(result.Data.JwtToken) as JwtSecurityToken;

            Assert.That(tokens?.ValidTo.ToString(CultureInfo.InvariantCulture),
                Is.EqualTo(validTo.ToString(CultureInfo.InvariantCulture).Substring(0, 19)));
            Assert.That(tokens?.Audiences.ToList()[0], Is.EqualTo(jwt.Audience));
            Assert.That(tokens?.Issuer, Is.EqualTo(jwt.Issuer));
            Assert.That(tokens?.Claims.SingleOrDefault(c => c.Type == "Id")?.Value, Is.EqualTo(_user.Id));
            Assert.That(tokens?.Claims.SingleOrDefault(c => c.Type == "Email")?.Value, Is.EqualTo(_user.Email));
            Assert.That(tokens?.Claims.SingleOrDefault(c => c.Type == "UserName")?.Value, Is.EqualTo(_user.UserName));
            Assert.That(tokens?.Claims.SingleOrDefault(c => c.Type == "SessionId")?.Value, Is.EqualTo(_user.SessionId));
            Assert.That(result.Data.User.ToString(), Is.EqualTo(_user.ToString()));
        }
    }
}