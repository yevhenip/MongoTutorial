using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Warehouse.Api.Users.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Messaging.Sender;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Tests.Business
{
    [TestFixture]
    public class UserServiceTests
    {
        private UserDto _user;

        private readonly User _dbUser = new()
        {
            Id = "a", Email = "a", Phone = "a", FullName = "a", RegistrationDateTime = DateTime.UtcNow,
            PasswordHash = "a", UserName = "a", Roles = new List<string> {"User"}, SessionId = "a"
        };

        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
        private readonly Mock<IOptions<CacheUserSettings>> _options = new();
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IFileService> _fileService = new();
        private readonly Mock<IDistributedCache> _cache = new();
        private readonly Mock<IMapper> _mapper = new();
        private readonly Mock<ISender> _sender = new();

        private UserService _userService;

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            _options.Setup(opt => opt.Value).Returns(new CacheUserSettings
                {AbsoluteExpiration = 1, SlidingExpiration = 1});
            _userService = new(_refreshTokenRepository.Object, _userRepository.Object, _options.Object,
                _cache.Object, _mapper.Object, _fileService.Object, _sender.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _user = new("a", "a", "a", "a", DateTime.UtcNow, "a", "a", new List<string> {"User"}, "a");
        }

        [Test]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfUsers()
        {
            var users = ConfigureGetAll();

            var result = await _userService.GetAllAsync();

            Assert.That(result.Data, Is.EqualTo(users));
        }

        [Test]
        public async Task GetAsync_WhenCalledWithUnCashedUser_ReturnsUser()
        {
            ConfigureGet(_dbUser, _dbUser);

            var result = await _userService.GetAsync(_dbUser.Id);

            Assert.That(result.Data, Is.EqualTo(_user));
        }

        [Test]
        public async Task GetAsync_WhenCalledWithFiledUser_ReturnsUser()
        {
            ConfigureGet(null, _dbUser);

            var result = await _userService.GetAsync(_dbUser.Id);

            Assert.That(result.Data, Is.EqualTo(_user));
        }

        [Test]
        public void GetAsync_WhenCalledWithUndefinedUser_ThrowsResultOfUserException()
        {
            ConfigureGet(null, null);

            Assert.ThrowsAsync<Result<User>>(async () => await _userService.GetAsync(_dbUser.Id));
        }

        [Test]
        public async Task CreateAsync_WhenCalledWithUniqueData_ReturnsUser()
        {
            var user = ConfigureCreate("b", "b");

            var result = await _userService.CreateAsync(_dbUser);

            Assert.That(result.Data, Is.EqualTo(_user));
            user.UserName = "a";
            user.Email = "a";
        }

        [Test]
        public void CreateAsync_WhenCalledWithDuplicatedEmail_ThrowsResultOfUserDtoException()
        {
            var user = ConfigureCreate("a", "b");

            Assert.ThrowsAsync<Result<UserDto>>(async () => await _userService.CreateAsync(_dbUser));
            user.UserName = "a";
        }

        [Test]
        public void CreateAsync_WhenCalledWithDuplicatedUserName_ThrowsResultOfUserDtoException()
        {
            var user = ConfigureCreate("b", "a");

            Assert.ThrowsAsync<Result<UserDto>>(async () => await _userService.CreateAsync(_dbUser));
            user.Email = "a";
        }

        [Test]
        public async Task UpdateAsync_WhenCalledWithUser_ReturnsUser()
        {
            await _userService.UpdateAsync(_dbUser);
        }

        [Test]
        public async Task UpdateAsync_WhenWithCalledUnCashedUser_ReturnsUser()
        {
            var user = ConfigureUpdate(_dbUser, null);

            var result = await _userService.UpdateAsync(_dbUser.Id, user, "a");

            Assert.That(result.Data, Is.EqualTo(_user));
        }

        [Test]
        public async Task UpdateAsync_WhenWithCalledUnFiledUser_ReturnsUser()
        {
            var user = ConfigureUpdate(null, _dbUser);

            var result = await _userService.UpdateAsync(_dbUser.Id, user, "a");

            Assert.That(result.Data, Is.EqualTo(_user));
        }

        [Test]
        public void UpdateAsync_WhenWithCalledUndefinedUser_ThrowsResultOfUserException()
        {
            var user = ConfigureUpdate(null, null);

            Assert.ThrowsAsync<Result<User>>(async () => await _userService.UpdateAsync(_dbUser.Id, user, "a"));
        }

        [Test]
        public async Task DeleteAsync_WhenCalledWithUnCachedUserAndExistingId_ReturnsNull()
        {
            ConfigureDelete(_dbUser);

            var result = await _userService.DeleteAsync(_dbUser.Id);

            Assert.That(result.Data, Is.EqualTo(null));
        }

        [Test]
        public void DeleteAsync_WhenCalledWithUnCachedUserAndUnExistingId_ThrowsResultOfUserException()
        {
            ConfigureDelete(null);

            Assert.ThrowsAsync<Result<User>>(async () => await _userService.DeleteAsync(_dbUser.Id));
        }

        [Test]
        public async Task GetRangeByRoleAsync_WhenCalled_ReturnsUsers()
        {
            var users = ConfigureGetAll();

            var result = await _userService.GetRangeByRoleAsync(It.IsAny<string>());

            Assert.That(result.Data, Is.EqualTo(users));
        }
        
        [Test]
        public async Task GetPageAsync_WhenCalled_ReturnsPageDateDtoOfUserDto()
        {
            var expected = ConfigureGetPage();

            var result = await _userService.GetPageAsync(1, 1);

            Assert.That(result.Data, Is.EqualTo(expected));
        }

        private List<UserDto> ConfigureGetAll()
        {
            List<UserDto> users = new() {_user};
            List<User> usersFromDb = new() {_dbUser};
            _userRepository.Setup(ur => ur.GetRangeByRoleAsync(It.IsAny<string>())).ReturnsAsync(usersFromDb);
            _userRepository.Setup(ur => ur.GetAllAsync()).ReturnsAsync(usersFromDb);
            _mapper.Setup(m => m.Map<List<UserDto>>(usersFromDb)).Returns(users);
            return users;
        }

        private void ConfigureGet(User dbUser, User fileUser)
        {
            _userRepository.Setup(ur => ur.GetAsync(_dbUser.Id)).ReturnsAsync(dbUser);
            _fileService.Setup(fs => fs.ReadFromFileAsync<User>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileUser);
            _mapper.Setup(m => m.Map<UserDto>(_dbUser)).Returns(_user);
        }

        private User ConfigureCreate(string email, string userName)
        {
            var user = JsonSerializer.Deserialize<User>(JsonSerializer.Serialize(_dbUser));
            user.Email = email;
            user.UserName = userName;
            List<User> usersFromDb = new() {user};
            _userRepository.Setup(ur => ur.GetAllAsync()).ReturnsAsync(usersFromDb);
            _mapper.Setup(m => m.Map<User>(_user)).Returns(_dbUser);
            _mapper.Setup(m => m.Map<UserDto>(_dbUser)).Returns(_user);
            return user;
        }

        private UserModelDto ConfigureUpdate(User dbUser, User fileUser)
        {
            UserModelDto user = new("a", "a", "a", "a");
            _userRepository.Setup(ur => ur.GetAsync(_dbUser.Id)).ReturnsAsync(dbUser);
            _fileService.Setup(fs => fs.ReadFromFileAsync<User>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(fileUser);
            _mapper.Setup(m => m.Map<UserDto>(user)).Returns(_user);

            return user;
        }

        private void ConfigureDelete(User user)
        {
            _userRepository.Setup(ur => ur.GetAsync(_dbUser.Id)).ReturnsAsync(user);
            _refreshTokenRepository.Setup(tr => tr.GetByUserIdAsync(_dbUser.Id))
                .ReturnsAsync(new RefreshToken
                {
                    Id = "a", Token = "a", User = _dbUser, DateCreated = DateTime.Today,
                    DateExpires = new DateTime(2222, 2, 2)
                });
        }
        
        private PageDataDto<UserDto> ConfigureGetPage()
        {
            List<UserDto> userDtos = new();
            List<User> users = new();
            PageDataDto<UserDto> expected = new(userDtos, 1);
            _userRepository.Setup(ur => ur.GetPageAsync(1, 1))
                .ReturnsAsync(users);
            _userRepository.Setup(ur => ur.GetCountAsync())
                .ReturnsAsync(1);
            _mapper.Setup(m => m.Map<List<UserDto>>(users)).Returns(userDtos);
            return expected;
        }
    }
}