using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Polly;
using Polly.CircuitBreaker;
using Warehouse.Core.DTO;
using Warehouse.Core.Settings;

namespace Warehouse.Core.Interfaces.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
    {
        private readonly IMongoCollection<TEntity> _collection;
        private readonly AsyncCircuitBreakerPolicy _dbPolicy;

        protected Repository(IMongoCollection<TEntity> collection, PollySettings pollySettings)
        {
            _collection = collection;
            _dbPolicy = Policy.Handle<TimeoutException>()
                .CircuitBreakerAsync(pollySettings.RepeatedTimes,
                    TimeSpan.FromSeconds(Math.Pow(pollySettings.RepeatedDelay, 2)));
        }

        public Task<List<TEntity>> GetRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.Find(predicate).ToListAsync());
        }

        public Task<List<TEntity>> GetPageAsync(int page, int pageSize)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync());
        }

        public Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.CountDocumentsAsync(predicate));
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.Find(predicate).SingleOrDefaultAsync());
        }

        public Task CreateAsync(TEntity item)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.InsertOneAsync(item));
        }

        public Task CreateRangeAsync(IEnumerable<TEntity> item)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.InsertManyAsync(item));
        }

        public Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity item)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.ReplaceOneAsync(predicate, item));
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbPolicy.ExecuteAsync(() => _collection.FindOneAndDeleteAsync(predicate));
        }

        public Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbPolicy.ExecuteAsync(async () =>
                await _collection.Find(predicate).FirstOrDefaultAsync() is not null);
        }

        public Task<List<GroupData>> GroupByAsync(Expression<Func<TEntity, string>> predicate)
        {
            return _dbPolicy.ExecuteAsync(() =>
                Task.FromResult(_collection.AsQueryable()
                    .OrderBy(predicate).GroupBy(predicate)
                    .Select(g => new GroupData(g.Key, g.Sum(_ => 1))).ToList()));
        }
    }
}