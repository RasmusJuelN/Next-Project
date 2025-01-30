using System.Linq.Expressions;

namespace Database.Repository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    public Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    public Task<TEntity> AddAsync(TEntity entity);

    public Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);

    public Task<TEntity> UpdateAsync(TEntity entity);

    public Task<TEntity> DeleteAsync(TEntity entity);

    public Task<IEnumerable<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    public Task<IEnumerable<TEntity>> DeleteRangeAsync(IEnumerable<TEntity> entities);
}
