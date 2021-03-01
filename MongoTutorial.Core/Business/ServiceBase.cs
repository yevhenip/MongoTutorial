using System.Net;
using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using MongoTutorial.Core.Common;

namespace MongoTutorial.Core.Business
{
    public abstract class ServiceBase<T> where T : class
    {
        protected readonly IDistributedCache DistributedCache;
        protected readonly IMapper Mapper;

        protected ServiceBase(IDistributedCache distributedCache, IMapper mapper)
        {
            DistributedCache = distributedCache;
            Mapper = mapper;
        }

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