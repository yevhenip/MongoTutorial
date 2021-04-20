using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Warehouse.Core.DTO.Log;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Logs
{
    public class CreatedLogHandler : IConsumeAsync<LogDto>
    {
        private readonly ILogService _logService;

        public CreatedLogHandler(ILogService logService)
        {
            _logService = logService;
        }

        public async Task ConsumeAsync(LogDto message, CancellationToken cancellationToken = new CancellationToken())
        {
            await _logService.CreateAsync(message);
        }
    }
}