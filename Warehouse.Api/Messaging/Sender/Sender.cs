using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Core.Interfaces.Messaging.Sender;

namespace Warehouse.Api.Messaging.Sender
{
    public class Sender : ISender
    {
        private readonly IConnection _connection;

        public Sender(IConnection connection)
        {
            _connection = connection;
        }

        public Task SendMessage<T>(T item, string queue)
        {
            return Task.Run(() =>
            {
                using var channel = _connection.CreateModel();
                channel.QueueDeclare(queue, false, false, true, null);

                var json = JsonSerializer.Serialize(item);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish("", queue, null, body);
            });
        }
    }
}