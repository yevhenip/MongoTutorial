using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Warehouse.Core.Common;
using Warehouse.Core.Interfaces.Messaging.Sender;
using Warehouse.Core.Interfaces.Services;

namespace Warehouse.Api.Business
{
    public abstract class ServiceBase<T> where T : class
    {
        protected readonly IDistributedCache DistributedCache;
        protected readonly IFileService FileService;
        protected readonly IMapper Mapper;
        protected readonly ISender Sender;

        protected readonly JsonSerializerOptions JsonSerializerOptions =
            new() {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};


        protected ServiceBase(IMapper mapper = null, IDistributedCache distributedCache = null,
            ISender sender = null, IFileService fileService = null)
        {
            DistributedCache = distributedCache;
            FileService = fileService;
            Sender = sender;
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