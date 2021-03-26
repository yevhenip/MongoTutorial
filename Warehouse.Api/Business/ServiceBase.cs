using System.Net;
using AutoMapper;
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

        protected ServiceBase(IDistributedCache distributedCache, IMapper mapper, IFileService fileService)
        {
            DistributedCache = distributedCache;
            FileService = fileService;
            Mapper = mapper;
        }

        /// <summary>
        /// Checks item fo being null and throws exception if it is true
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="Result"></exception>
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