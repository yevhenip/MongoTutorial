using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Warehouse.Core.Settings;

namespace Warehouse.Api.Common.EventBus
{
    public class EventBusInitializationBackgroundService : BackgroundService
    {
        private readonly IBus _bus;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventSubscriptionSettings _settings;

        public EventBusInitializationBackgroundService(IBus bus, IServiceProvider serviceProvider,
            IOptions<EventSubscriptionSettings> options)
        {
            _bus = bus;
            _serviceProvider = serviceProvider;
            _settings = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_settings.EventHandlersAssemblies is not null)
            {
                var subscriber = new AutoSubscriber(_bus, _settings.EventHandlersAssemblies.ToString())
                {
                    AutoSubscriberMessageDispatcher = new MicrosoftDiMessageDispatcher(_serviceProvider)
                };
                await subscriber.SubscribeAsync(new[] {_settings.EventHandlersAssemblies}, stoppingToken);
            }
        }
    }
}