using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using MediatR;
using Warehouse.Api.Logs.Commands;
using Warehouse.Core.DTO.Log;

namespace Warehouse.Api.Logs.Receivers
{
    public class CreatedLogHandler : IConsumeAsync<LogDto>
    {
        private readonly IMediator _mediator;

        public CreatedLogHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task ConsumeAsync(LogDto message, CancellationToken cancellationToken = new())
        {
            await _mediator.Send(new CreateLogCommand(message), cancellationToken);
        }
    }
}