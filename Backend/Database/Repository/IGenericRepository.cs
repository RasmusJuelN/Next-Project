using System.Linq.Expressions;

namespace Database.Repository;

public interface IGenericRepository<TEntity> where TEntity : class
{
    public Task<TEntity> GetAsync(int id);
    
    public Task<TEntity> GetAsync(Guid id);

    public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);

    public Task<IEnumerable<TEntity>> GetAllAsync();

    public Task<TEntity> AddAsync(TEntity entity);

    public Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);

    public Task<TEntity> UpdateAsync(TEntity entity);

    public Task<TEntity> DeleteAsync(int id);

    public Task<TEntity> DeleteAsync(Guid id);

    public Task<TEntity> DeleteAsync(TEntity entity);

    public Task<IEnumerable<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate);
}
