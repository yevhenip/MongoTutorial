using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using RabbitMQ.Client.Exceptions;
using StackExchange.Redis;
using Warehouse.Core.Common;
using Warehouse.Core.Interfaces.Services;
using Warehouse.Core.Settings;

namespace Warehouse.Api.Business
{
    public abstract class ServiceBase<T> where T : class
    {
        protected readonly IDistributedCache DistributedCache;
        protected readonly IFileService FileService;
        protected readonly IMapper Mapper;
        protected readonly IBus Bus;

        protected readonly JsonSerializerOptions JsonSerializerOptions =
            new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        protected readonly AsyncCircuitBreakerPolicy CachingPolicy;
        protected readonly AsyncRetryPolicy DbPolicy;
        protected readonly AsyncRetryPolicy RabbitPolicy;

        protected ServiceBase(IMapper mapper = default, IBus bus = default, PollySettings pollySettings = default,
            IDistributedCache distributedCache = default, IFileService fileService = default)
        {
            DistributedCache = distributedCache;
            FileService = fileService;
            Bus = bus;
            Mapper = mapper;
            DbPolicy = Policy.Handle<TimeoutException>()
                .WaitAndRetryAsync(pollySettings.RepeatedTimes,
                    ra => TimeSpan.FromSeconds(Math.Pow(pollySettings.RepeatedDelay, ra)));

            RabbitPolicy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetryAsync(pollySettings.RepeatedTimes,
                    ra => TimeSpan.FromSeconds(Math.Pow(pollySettings.RepeatedDelay, ra)));
            
            CachingPolicy = Policy.Handle<RedisConnectionException>()
                .Or<RedisTimeoutException>()
                .CircuitBreakerAsync(pollySettings.RepeatedTimes, TimeSpan.FromSeconds(pollySettings.RepeatedDelay));
        }

        /// <summary>
        /// Checks item fo being null and throws exception if it is true
        /// </summary>
        /// <param name="item"></param>
        protected void CheckForNull(T item)
        {
            if (item == null)
            {
                var typeName = typeof(T).Name;
                throw Result<T>.Failure("id", $"{typeName} doesn't exists", HttpStatusCode.BadRequest);
            }
        }
    }
}