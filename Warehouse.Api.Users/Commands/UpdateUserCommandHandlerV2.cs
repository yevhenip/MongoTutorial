using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings.CacheSettings;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Commands
{
    public record UpdateUserCommandV2(User User) : IRequest;

    public class UpdateUserCommandV2Handler : IRequestHandler<UpdateUserCommandV2>
    {
        private readonly CacheUserSettings _userSettings;
        private readonly ICacheService _cacheService;
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandV2Handler(IOptions<CacheUserSettings> userSettings, ICacheService cacheService,
            IUserRepository userRepository)
        {
            _userSettings = userSettings.Value;
            _cacheService = cacheService;
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(UpdateUserCommandV2 request, CancellationToken cancellationToken)
        {
            var cacheKey = $"User-{request.User.Id}";

            await _userRepository.UpdateAsync(u => u.Id == request.User.Id, request.User);
            await _cacheService.UpdateAsync(cacheKey, request.User, _userSettings);
            return Unit.Value;
        }
    }
}