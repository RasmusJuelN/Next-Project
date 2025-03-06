using System.Linq.Expressions;

namespace Database.Interfaces;

internal interface IGenericRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);
    
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    Task AddAsync(TEntity entity);

    Task AddRangeAsync(List<TEntity> entities);

    void Delete(TEntity entity);

    void DeleteRange(List<TEntity> entities);
    
    IQueryable<TEntity> GetAsQueryable();
    
    int Count(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    bool Exists(Expression<Func<TEntity, bool>> predicate);
}
