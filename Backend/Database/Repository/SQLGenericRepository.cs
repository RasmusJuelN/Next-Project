using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Database.Repository;

public class SQLGenericRepository<TEntity>(Context context) : IGenericRepository<TEntity> where TEntity : class
{
    // TODO: Create custom exceptions and include logging
    private readonly Context _context = context;

    public async Task<TEntity> GetAsync(int id)
    {
        return await _context.Set<TEntity>().FindAsync(id) ?? throw new Exception("No matching entry found");
    }

    public async Task<TEntity> GetAsync(Guid id)
    {
        return await _context.Set<TEntity>().FindAsync(id) ?? throw new Exception("No matching entry found");
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().Where(predicate).ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
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

    public async Task<TEntity> DeleteAsync(int id)
    {
        TEntity entity = await GetAsync(id);
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> DeleteAsync(Guid id)
    {
        TEntity entity = await GetAsync(id);
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<IEnumerable<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        IEnumerable<TEntity> entities = await GetAsync(predicate);
        _context.Set<TEntity>().RemoveRange(entities);
        await _context.SaveChangesAsync();

        return entities;
    }
}
