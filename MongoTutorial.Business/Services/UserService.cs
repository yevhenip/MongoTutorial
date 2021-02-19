using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MongoTutorial.Core.Common;
using MongoTutorial.Core.DTO.Users;
using MongoTutorial.Core.Interfaces.Repositories;
using MongoTutorial.Core.Interfaces.Services;
using MongoTutorial.Domain;

namespace MongoTutorial.Business.Services
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IRefreshTokenRepository tokenRepository, IUserRepository userRepository, IMapper mapper)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<UserDto>>> GetAllAsync()
        {
            var usersFromDb = await _userRepository.GetAllAsync();
            var users = _mapper.Map<List<UserDto>>(usersFromDb);

            return Result<List<UserDto>>.Success(users);
        }

        public async Task<Result<UserDto>> GetAsync(string id)
        {
            var userInDb = await _userRepository.GetAsync(id);
            CheckForNull(userInDb);

            var user = _mapper.Map<UserDto>(userInDb);

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> GetByUserNameAsync(string userName)
        {
            var userInDb = await _userRepository.GetByUserNameAsync(userName);
            CheckForNull(userInDb);

            var user = _mapper.Map<UserDto>(userInDb);

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> CreateAsync(UserDto user)
        {
            await IsValid(user);

            var userToDb = _mapper.Map<User>(user);
            await _userRepository.CreateAsync(userToDb);

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> UpdateAsync(string userId, UserModelDto user)
        {
            var userInDb = await _userRepository.GetAsync(userId);
            CheckForNull(userInDb);

            var userDto = _mapper.Map<UserDto>(user) with {Id = userId};
            await IsValid(userDto);
            userInDb = _mapper.Map<User>(userDto);
            var token = await _tokenRepository.GetByUserIdAsync(userId);
            
            if (token is not null)
            {
                token.User = userInDb;
                await _tokenRepository.UpdateAsync(token);
            }

            await _userRepository.UpdateAsync(userInDb);
            return Result<UserDto>.Success(userDto);
        }

        public async Task<Result<UserDto>> UpdateAsync(UserDto user)
        {
            var userToDb = _mapper.Map<User>(user);
            await _userRepository.UpdateAsync(userToDb);
            return Result<UserDto>.Success(user);
        }

        public async Task<Result<object>> DeleteAsync(string id)
        {
            var userInDb = await _userRepository.GetAsync(id);
            CheckForNull(userInDb);
            var token = await _tokenRepository.GetByUserIdAsync(id);
            if (token is not null)
            {
                await _tokenRepository.DeleteAsync(token.Id);
            }

            await _userRepository.DeleteAsync(id);
            return Result<object>.Success();
        }

        public async Task<Result<List<UserDto>>> GetRangeByRoleAsync(string roleName)
        {
            var usersInDb = await _userRepository.GetRangeByRoleAsync(roleName);
            var users = _mapper.Map<List<UserDto>>(usersInDb);
            
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