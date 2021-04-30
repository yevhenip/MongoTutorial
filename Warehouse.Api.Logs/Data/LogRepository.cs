using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Core.Settings;
using Warehouse.Domain;

namespace Warehouse.Api.Logs.Data
{
    public class LogRepository : Repository<Log>, ILogRepository
    {
        public LogRepository(IMongoClient client, IOptions<PollySettings> pollySettings)
            : base(client.GetDatabase("Warehouse_log").GetCollection<Log>("log"), pollySettings.Value)
        {
        }
    }
}