using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Warehouse.Core.DTO;

namespace Warehouse.Core.Interfaces.Repositories
{
    public interface IRepository<TEntity>
    {
        Task<List<TEntity>> GetRangeAsync(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> GetPageAsync(int page, int pageSize);

        Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate);

        Task CreateAsync(TEntity item);

        Task CreateRangeAsync(IEnumerable<TEntity> item);

        Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity item);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate);

        Task<List<GroupData>> GroupByAsync(Expression<Func<TEntity, string>> orderPredicate);
    }
}