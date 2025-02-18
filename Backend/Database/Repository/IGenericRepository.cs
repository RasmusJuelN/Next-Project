using System.Linq.Expressions;

namespace Database.Repository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);
    
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    Task<TEntity> AddAsync(TEntity entity);

    Task<List<TEntity>> AddRangeAsync(List<TEntity> entities);

    Task<TEntity> UpdateAsync(TEntity entity, TEntity existingEntity);

    Task<TEntity> PatchAsync(TEntity existingEntity, object newValues);
    
    Task<List<TEntity>> PatchAsync(Expression<Func<TEntity, bool>> predicate, object newValues);

    Task<TEntity> DeleteAsync(TEntity entity);

    Task<List<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

    Task<List<TEntity>> DeleteRangeAsync(List<TEntity> entities);
    
    IQueryable<TEntity> GetAsQueryable();
    
    int GetCount(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    bool Exists(Expression<Func<TEntity, bool>> predicate);
}
