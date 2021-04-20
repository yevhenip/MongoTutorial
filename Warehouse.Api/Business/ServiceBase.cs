using System.Net;
using System.Text.Json;
using AutoMapper;
using EasyNetQ;
using Microsoft.Extensions.Caching.Distributed;
using Warehouse.Core.Common;
using Warehouse.Core.Interfaces.Services;

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


        protected ServiceBase(IMapper mapper = null, IDistributedCache distributedCache = null,
            IBus bus = null, IFileService fileService = null)
        {
            DistributedCache = distributedCache;
            FileService = fileService;
            Bus = bus;
            Mapper = mapper;
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