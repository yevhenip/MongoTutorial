using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Core.Business;
using Warehouse.Core.Common;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Extensions;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Business
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly string _path = Directory.GetCurrentDirectory() + @"\..\Warehouse.Api\wwwroot\Users\";
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly CacheUserSettings _userSettings;
        private readonly IUserRepository _userRepository;

        public UserService(IRefreshTokenRepository tokenRepository, IUserRepository userRepository,
            IOptions<CacheUserSettings> userSettings, IDistributedCache distributedCache, IMapper mapper,
            IFileService fileService) : base(distributedCache, mapper, fileService)
        {
            _userSettings = userSettings.Value;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<List<UserDto>>> GetAllAsync()
        {
            var usersFromDb = await _userRepository.GetAllAsync();
            var users = Mapper.Map<List<UserDto>>(usersFromDb);

            return Result<List<UserDto>>.Success(users);
        }

        public async Task<Result<UserDto>> GetAsync(string id)
        {
            var cacheKey = $"User-{id}";
            var cache = await DistributedCache.GetStringAsync(cacheKey);
            UserDto user;
            if (cache.TryGetValue<User>(out var cachedUser))
            {
                user = Mapper.Map<UserDto>(cachedUser);

                return Result<UserDto>.Success(user);
            }

            var userInDb = await _userRepository.GetAsync(id);
            if (userInDb is not null)
            {
                await DistributedCache.SetCacheAsync(cacheKey, userInDb, _userSettings);

                user = Mapper.Map<UserDto>(userInDb);

                return Result<UserDto>.Success(user);
            }

            userInDb = await FileService.ReadFromFileAsync<User>(_path, cacheKey);
            CheckForNull(userInDb);

            user = Mapper.Map<UserDto>(userInDb);

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> GetByUserNameAsync(string userName)
        {
            var userInDb = await _userRepository.GetByUserNameAsync(userName);
            CheckForNull(userInDb);

            var user = Mapper.Map<UserDto>(userInDb);

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> CreateAsync(UserDto user)
        {
            await IsValid(user);

            var userToDb = Mapper.Map<User>(user);
            await _userRepository.CreateAsync(userToDb);

            var cacheKey = $"User-{userToDb.Id}";
            await DistributedCache.SetCacheAsync(cacheKey, userToDb, _userSettings);
            await FileService.WriteToFileAsync(userToDb, _path, cacheKey);

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> UpdateAsync(string userId, UserModelDto user)
        {
            var cacheKey = $"User-{userId}";
            User userInDb;
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                userInDb = await _userRepository.GetAsync(userId) ??
                           await FileService.ReadFromFileAsync<User>(_path, cacheKey);
                CheckForNull(userInDb);
            }

            var userDto = Mapper.Map<UserDto>(user) with {Id = userId};
            userInDb = Mapper.Map<User>(userDto);
            var token = await _tokenRepository.GetByUserIdAsync(userId);

            if (token is not null)
            {
                token.User = userInDb;
                await _tokenRepository.UpdateAsync(token);
            }

            await _userRepository.UpdateAsync(userInDb);
            await DistributedCache.UpdateAsync(cacheKey, userInDb);
            await FileService.WriteToFileAsync(userInDb, _path, cacheKey);

            return Result<UserDto>.Success(userDto);
        }

        public async Task UpdateAsync(User user)
        {
            var cacheKey = $"Product-{user.Id}";

            await _userRepository.UpdateAsync(user);
            await DistributedCache.UpdateAsync(cacheKey, user);
            await FileService.WriteToFileAsync(user, _path, cacheKey);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"User-{id}";
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                var userInDb = await _userRepository.GetAsync(id);
                CheckForNull(userInDb);
            }

            var token = await _tokenRepository.GetByUserIdAsync(id);
            if (token is not null)
            {
                await _tokenRepository.DeleteAsync(token.Id);
            }

            await _userRepository.DeleteAsync(id);
            await DistributedCache.RemoveAsync(cacheKey);
            await FileService.DeleteFileAsync(_path, cacheKey);

            return Result<object>.Success();
        }

        public async Task<Result<List<UserDto>>> GetRangeByRoleAsync(string roleName)
        {
            var usersInDb = await _userRepository.GetRangeByRoleAsync(roleName);
            var users = Mapper.Map<List<UserDto>>(usersInDb);

            return Result<List<UserDto>>.Success(users);
        }

        private async Task IsValid(UserDto user)
        {
            var users = await _userRepository.GetAllAsync();
            if (users.Any(u => u.Email == user.Email))
            {
                throw Result<UserDto>.Failure("email", "User with such email already exists",
                    HttpStatusCode.BadRequest);
            }

            if (users.Any(u => u.UserName == user.UserName))
            {
                throw Result<UserDto>.Failure("userName", "User with such userName already exists",
                    HttpStatusCode.BadRequest);
            }
        }
    }
}