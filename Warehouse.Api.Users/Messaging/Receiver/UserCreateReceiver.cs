using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Extensions;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.DTO.Users;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Users.Messaging.Receiver
{
    public class UserCreateReceiver : Receiver<UserDto>
    {
        private const string Queue = Queues.CreateUserQueue;
        private readonly IUserService _userService;

        public UserCreateReceiver(IUserService userService, IConnection connection) : base(connection, Queue)
        {
            _userService = userService;
        }

        protected override async Task HandleMessage(UserDto user)
        {
            await _userService.CreateAsync(user);
        }
    }
}