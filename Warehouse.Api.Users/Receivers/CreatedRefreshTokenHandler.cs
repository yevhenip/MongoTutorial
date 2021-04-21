using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users.Receivers
{
    public class CreatedRefreshTokenHandler : IConsumeAsync<CreatedToken>
    {
        private readonly IUserService _userService;

        public CreatedRefreshTokenHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task ConsumeAsync(CreatedToken message, CancellationToken cancellationToken = new())
        {
            await _userService.CreateTokenAsync(message);
        }
    }
}