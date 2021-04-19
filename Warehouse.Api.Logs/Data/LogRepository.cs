using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Warehouse.Core.Interfaces.Repositories;
using Warehouse.Domain;

namespace Warehouse.Api.Logs.Data
{
    public class LogRepository : ILogRepository
    {
        private readonly IMongoCollection<Log> _logCollection;

        public LogRepository(IMongoClient client)
        {
            var db = client.GetDatabase("Warehouse_log");
            _logCollection = db.GetCollection<Log>("log");
        }

        public Task<List<Log>> GetAllAsync()
        {
            return _logCollection.Find(_ => true).ToListAsync();
        }

        public Task<List<Log>> GetActualAsync()
        {
            return _logCollection.Find(l => l.ActionDate >= DateTime.UtcNow.AddDays(-1)).ToListAsync();
        }

        public Task<Log> GetAsync(string id)
        {
            return _logCollection.Find(p => p.Id == id).SingleOrDefaultAsync();
        }

        public Task CreateAsync(Log log)
        {
            return _logCollection.InsertOneAsync(log);
        }


        public Task DeleteAsync(string id)
        {
            return _logCollection.FindOneAndDeleteAsync(p => p.Id == id);
        }
    }
}