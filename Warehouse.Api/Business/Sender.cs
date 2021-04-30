
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings;

namespace Warehouse.Api.Business
{
    public class Sender : ISender
    {
        private readonly IBus _bus;
        private readonly AsyncRetryPolicy _retryPolicy;
        public Sender(IBus bus, IOptions<PollySettings> pollySettings)
        {
            _bus = bus;
            _retryPolicy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(pollySettings.Value.RepeatedTimes,
                    ra => TimeSpan.FromSeconds(Math.Pow(pollySettings.Value.RepeatedDelay, ra)));
        }
        public Task PublishAsync<T>(T item, CancellationToken cancellationToken = default)
        {
            return _retryPolicy.ExecuteAsync(() => _bus.PubSub.PublishAsync(item, cancellationToken));
        }
    }
}