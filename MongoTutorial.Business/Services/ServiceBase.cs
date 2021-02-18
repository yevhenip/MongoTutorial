using System.Net;
using MongoTutorial.Core.Common;

namespace MongoTutorial.Business.Services
{
    public abstract class ServiceBase<T> where T : class
    {
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