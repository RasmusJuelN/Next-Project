using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repository;

public class SQLGenericRepository<TEntity>(Context context) : IGenericRepository<TEntity> where TEntity : class
{
    // TODO: Create custom exceptions and include logging
    private readonly Context _context = context;

    public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>().Where(predicate);

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return await query.ToListAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }
        
        return await query.ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
    {
        await _context.Set<TEntity>().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<List<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        List<TEntity> entities = await GetAsync(predicate);
        _context.Set<TEntity>().RemoveRange(entities);
        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task<List<TEntity>> DeleteRangeAsync(List<TEntity> entities)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        await _context.SaveChangesAsync();

        return entities;
    }
}
