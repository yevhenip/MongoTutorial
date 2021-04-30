using MediatR;

namespace Warehouse.Api.Base
{
    public abstract class ReceiverBase
    {
        protected readonly IMediator Mediator;

        protected ReceiverBase(IMediator mediator)
        {
            Mediator = mediator;
        }
    }
}