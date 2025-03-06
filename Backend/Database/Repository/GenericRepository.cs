using System.Linq.Expressions;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

internal class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    // TODO: Create custom exceptions and include logging
    private readonly Context _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly ILogger _logger;

    public GenericRepository(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return await query.SingleOrDefaultAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }
        
        return await query.ToListAsync();
    }

    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    public async Task AddRangeAsync(List<TEntity> entities) => await _dbSet.AddRangeAsync(entities);

    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    public void DeleteRange(List<TEntity> entities) => _dbSet.RemoveRange(entities);

    public IQueryable<TEntity> GetAsQueryable() => _dbSet;

    public int Count(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return query.Count();
    }

    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        int entityCount = Count(predicate);

        return entityCount > 0;
    }
}
