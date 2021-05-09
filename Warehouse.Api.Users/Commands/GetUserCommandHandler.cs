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
    public record GetUserCommand(string Id) : IRequest<Result<UserDto>>;

    public class GetUserCommandHandler : IRequestHandler<GetUserCommand, Result<UserDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;
        private readonly CacheUserSettings _userSettings;

        public GetUserCommandHandler(IMapper mapper, ICacheService cacheService, IUserRepository userRepository,
            IOptions<CacheUserSettings> userSettings)
        {
            _mapper = mapper;
            _cacheService = cacheService;
            _userRepository = userRepository;
            _userSettings = userSettings.Value;
        }

        public async Task<Result<UserDto>> Handle(GetUserCommand request, CancellationToken cancellationToken)
        {
            var cacheKey = $"User-{request.Id}";
            var cache = await _cacheService.GetStringAsync(cacheKey);
            UserDto user;
            if (cache.TryGetValue<User>(out var cachedUser))
            {
                user = _mapper.Map<UserDto>(cachedUser);

                return Result<UserDto>.Success(user);
            }

            var userInDb = await _userRepository.GetAsync(u => u.Id == request.Id);

            userInDb.CheckForNull();
            await _cacheService.SetCacheAsync(cacheKey, userInDb, _userSettings);
            user = _mapper.Map<UserDto>(userInDb);

            return Result<UserDto>.Success(user);
        }
    }
}