using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Base;
using Warehouse.Api.Logs.Commands;
using Warehouse.Core.DTO.Log;

namespace Warehouse.Api.Logs.Receivers
{
    public class CreatedLogHandler : ReceiverBase, IConsumeAsync<LogDto>
    {
        public CreatedLogHandler(IMediator mediator) : base(mediator)
        {
        }

        public async Task ConsumeAsync(LogDto message, CancellationToken cancellationToken = new())
        {
            await Mediator.Send(new CreateLogCommand(message), cancellationToken);
        }
    }
}