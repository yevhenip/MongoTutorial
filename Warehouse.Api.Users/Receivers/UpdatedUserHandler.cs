using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Receivers
{
    public class UpdatedUserHandler : IConsumeAsync<User>
    {
        private readonly IUserService _userService;

        public UpdatedUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task ConsumeAsync(User message, CancellationToken cancellationToken = new())
        {
            await _userService.UpdateAsync(message);
        }
    }
}