using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Logs.Data
{
    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(IMongoClient client)
            : base(client.GetDatabase("Warehouse_log").GetCollection<Log>("log"))
        {
        }
    }
}