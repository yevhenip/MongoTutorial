using System.Threading.Tasks;
using RabbitMQ.Client;
using Warehouse.Api.Extensions;
using Warehouse.Api.Messaging.Receiver;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Logs.Messaging.Receiver
{
    public class CreateLogReceiver: Receiver<LogDto>
    {
        private const string Queue = Queues.CreateLog;
        private readonly ILogService _logService;


        public CreateLogReceiver(ILogService logService, IConnection connection) : base(connection, Queue)
        {
            _logService = logService;
        }

        protected override async Task HandleMessage(LogDto log)
        {
            await _logService.CreateAsync(log);
        }
    }
}