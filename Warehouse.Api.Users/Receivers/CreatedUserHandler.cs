using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users.Receivers
{
    public class CreatedUserHandler : IConsumeAsync<CreatedUser>
    {
        private readonly IUserService _userService;

        public CreatedUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task ConsumeAsync(CreatedUser message, CancellationToken cancellationToken = new())
        {
            await _userService.CreateAsync(message);
        }
    }
}