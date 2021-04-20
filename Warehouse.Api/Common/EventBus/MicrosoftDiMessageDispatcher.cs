using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.DependencyInjection;

namespace Warehouse.Api.Common.EventBus
{
    public class MicrosoftDiMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public MicrosoftDiMessageDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispatch<TMessage, TConsumer>(TMessage message, CancellationToken cancellationToken = new())
            where TMessage : class where TConsumer : class, IConsume<TMessage>
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetService<TConsumer>();
            consumer?.Consume(message, cancellationToken);
        }

        public async Task DispatchAsync<TMessage, TConsumer>(TMessage message,
            CancellationToken cancellationToken = new()) where TMessage : class
            where TConsumer : class, IConsumeAsync<TMessage>
        {
            using var scope = _serviceProvider.CreateScope();
            var consumer = scope.ServiceProvider.GetService<TConsumer>();
            await consumer?.ConsumeAsync(message, cancellationToken);
        }
    }
}