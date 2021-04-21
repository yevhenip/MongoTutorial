using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Warehouse.Api.Business;
using Warehouse.Api.Extensions;
using Warehouse.Core.Common;
using Warehouse.Core.DTO;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            IFileService fileService, IBus bus) : base(mapper, distributedCache, bus, fileService)
        {
            _userSettings = userSettings.Value;
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<List<UserDto>>> GetAllAsync()
        {
            var usersFromDb = await _userRepository.GetRangeAsync(_ => true);
            var users = Mapper.Map<List<UserDto>>(usersFromDb);

            return Result<List<UserDto>>.Success(users);
        }

        public async Task<Result<PageDataDto<UserDto>>> GetPageAsync(int page, int pageSize)
        {
            var usersInDb = await _userRepository.GetPageAsync(page, pageSize);
            var count = await _userRepository.GetCountAsync(_ => true);
            var users = Mapper.Map<List<UserDto>>(usersInDb);
            PageDataDto<UserDto> pageData = new(users, count);

            return Result<PageDataDto<UserDto>>.Success(pageData);
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

            var userInDb = await _userRepository.GetAsync(u => u.Id == id);
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
            var userInDb = await _userRepository.GetAsync(u => u.UserName == userName);
            CheckForNull(userInDb);

            var user = Mapper.Map<UserDto>(userInDb);

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> CreateAsync(CreatedUser user)
        {
            await IsValid(user.User);

            var userFromDb = Mapper.Map<UserDto>(user.User);
            await _userRepository.CreateAsync(user.User);

            var cacheKey = $"User-{user.User.Id}";
            await DistributedCache.SetCacheAsync(cacheKey, user, _userSettings);
            await FileService.WriteToFileAsync(user, _path, cacheKey);

            return Result<UserDto>.Success(userFromDb);
        }

        public async Task<Result<UserDto>> UpdateAsync(string userId, UserModelDto user, string userName)
        {
            var cacheKey = $"User-{userId}";
            User userInDb;
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                userInDb = await _userRepository.GetAsync(u => u.Id == userId) ??
                           await FileService.ReadFromFileAsync<User>(_path, cacheKey);
                CheckForNull(userInDb);
            }

            var userDto = Mapper.Map<UserDto>(user) with {Id = userId};
            userInDb = Mapper.Map<User>(userDto);
            var token = await _tokenRepository.GetAsync(t => t.User.Id == userId);
            LogDto log =
                new(Guid.NewGuid().ToString(), userName, "edited user", JsonSerializer.Serialize(userDto,
                    JsonSerializerOptions), DateTime.UtcNow);

            if (token is not null)
            {
                token.User = userInDb;
                await _tokenRepository.UpdateAsync(t => t.Id == token.Id, token);
            }

            await _userRepository.UpdateAsync(u => u.Id == userInDb.Id, userInDb);
            await DistributedCache.UpdateAsync(cacheKey, userInDb);
            await FileService.WriteToFileAsync(userInDb, _path, cacheKey);
            await Bus.PubSub.PublishAsync(log);

            return Result<UserDto>.Success(userDto);
        }

        public async Task UpdateAsync(UpdatedUser user)
        {
            var cacheKey = $"User-{user.User.Id}";

            await _userRepository.UpdateAsync(u => u.Id == user.User.Id, user.User);
            await DistributedCache.UpdateAsync(cacheKey, user.User);
            await FileService.WriteToFileAsync(user.User, _path, cacheKey);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var cacheKey = $"User-{id}";
            if (!await DistributedCache.IsExistsAsync(cacheKey))
            {
                var userInDb = await _userRepository.GetAsync(u => u.Id == id);
                CheckForNull(userInDb);
            }

            var token = await _tokenRepository.GetAsync(t => t.Id == id);
            if (token is not null)
            {
                await _tokenRepository.DeleteAsync(t => t.Id == token.Id);
                await Bus.PubSub.PublishAsync(token);
            }

            await _userRepository.DeleteAsync(u => u.Id == id);
            await DistributedCache.RemoveAsync(cacheKey);
            await FileService.DeleteFileAsync(_path, cacheKey);

            return Result<object>.Success();
        }

        public async Task CreateTokenAsync(CreatedToken token)
        {
            await _tokenRepository.CreateAsync(token.Token);
        }
        
        public async Task DeleteTokenAsync(DeletedToken token)
        {
            await _tokenRepository.DeleteAsync(t => t.Id == token.Id);
        }

        public async Task<Result<List<UserDto>>> GetRangeByRoleAsync(string roleName)
        {
            var usersInDb = await _userRepository.GetRangeAsync(u => u.Roles.Contains(roleName));
            var users = Mapper.Map<List<UserDto>>(usersInDb);

            return Result<List<UserDto>>.Success(users);
        }

        private async Task IsValid(User user)
        {
            var users = await _userRepository.GetRangeAsync(_ => true);
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