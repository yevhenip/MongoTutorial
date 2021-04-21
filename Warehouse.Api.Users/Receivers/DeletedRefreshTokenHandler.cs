using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Auth;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users.Receivers
{
    public class DeletedRefreshTokenHandler : IConsumeAsync<DeletedToken>
    {
        private readonly IUserService _userService;

        public DeletedRefreshTokenHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task ConsumeAsync(DeletedToken message, CancellationToken cancellationToken = new())
        {
            await _userService.DeleteTokenAsync(message);
        }
    }
}