using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users.Receivers
{
    public class UpdatedUserHandler : IConsumeAsync<UpdatedUser>
    {
        private readonly IUserService _userService;

        public UpdatedUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task ConsumeAsync(UpdatedUser message, CancellationToken cancellationToken = new())
        {
            await _userService.UpdateAsync(message);
        }
    }
}