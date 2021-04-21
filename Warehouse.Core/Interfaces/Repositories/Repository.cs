using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Warehouse.Core.Interfaces.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
    {
        private readonly IMongoCollection<TEntity> _collection;

        protected Repository(IMongoCollection<TEntity> collection)
        {
            _collection = collection;
        }

        public Task<List<TEntity>> GetRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.Find(predicate).ToListAsync();
        }

        public Task<List<TEntity>> GetPageAsync(int page, int pageSize)
        {
            return _collection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        public Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.CountDocumentsAsync(predicate);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.Find(predicate).SingleOrDefaultAsync();
        }

        public Task CreateAsync(TEntity item)
        {
            return _collection.InsertOneAsync(item);
        }

        public Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity item)
        {
            return _collection.ReplaceOneAsync(predicate, item);
        }

        public Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.FindOneAndDeleteAsync(predicate);
        }
    }
}