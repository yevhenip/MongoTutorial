using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Extensions;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Domain;

namespace Warehouse.Api.Users.Messaging.Receiver
{
    public class UserCreateReceiver : Receiver<User>
    {
        private const string Queue = Queues.CreateUserQueue;
        private readonly IUserService _userService;

        public UserCreateReceiver(IUserService userService, IConnection connection) : base(connection, Queue)
        {
            _userService = userService;
        }

        protected override async Task HandleMessage(User user)
        {
            await _userService.CreateAsync(user);
        }
    }
}