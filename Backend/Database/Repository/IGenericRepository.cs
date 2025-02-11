using System.Linq.Expressions;

namespace Database.Repository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    public Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);
    
    public Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    public Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    public Task<TEntity> AddAsync(TEntity entity);

    public Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);

    public Task<TEntity> UpdateAsync(TEntity entity);

    public Task<TEntity> DeleteAsync(TEntity entity);

    public Task<List<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    public Task<List<TEntity>> DeleteRangeAsync(List<TEntity> entities);
    
    public IQueryable<TEntity> GetAsQueryable();
    
    public int GetCount(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);
}
