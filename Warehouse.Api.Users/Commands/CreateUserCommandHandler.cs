using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Api.Common;
using Warehouse.Api.Extensions;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Commands
{
    public record CreateUserCommand(User User) : IRequest<Result<UserDto>>;

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IFileService _fileService;
        private readonly IUserRepository _userRepository;
        private readonly CacheUserSettings _userSettings;

        public CreateUserCommandHandler(IMapper mapper, ICacheService cacheService, IFileService fileService, 
            IOptions<CacheUserSettings> userSettings, IUserRepository userRepository)
        {
            _mapper = mapper;
            _cacheService = cacheService;
            _fileService = fileService;
            _userRepository = userRepository;
            _userSettings = userSettings.Value;
        }

        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            await IsValid(request.User);

            var userFromDb = _mapper.Map<UserDto>(request.User);
            await _userRepository.CreateAsync(request.User);

            var cacheKey = $"User-{request.User.Id}";
            await _cacheService.SetCacheAsync(cacheKey, request.User, _userSettings);
            await _fileService.WriteToFileAsync(request.User, CommandExtensions.UserFolderPath, cacheKey);

            return Result<UserDto>.Success(userFromDb);
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