using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class SQLGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    // TODO: Create custom exceptions and include logging
    private readonly Context _context;
    private readonly ILogger _logger;

    public SQLGenericRepository(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _logger = loggerFactory.CreateLogger(GetType());
    }

    public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = GetDbSet().Where(predicate);

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = GetDbSet().Where(predicate);

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return await query.SingleOrDefaultAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = GetDbSet();

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }
        
        return await query.ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await GetDbSet().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
    {
        await GetDbSet().AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task<TEntity> UpdateAsync(TEntity updatedEntity, TEntity existingEntity)
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> PatchAsync(TEntity existingEntity, object newValues)
    {
        SQLGenericRepository<TEntity>.Patch(existingEntity, newValues);
        await _context.SaveChangesAsync();

        return existingEntity;
    }

    public async Task<List<TEntity>> PatchAsync(Expression<Func<TEntity, bool>> predicate, object newValues)
    {
        List<TEntity> existingEntities = await GetAsync(predicate);

        foreach (TEntity existingEntity in existingEntities)
        {
            SQLGenericRepository<TEntity>.Patch(existingEntity, newValues);
        }

        await _context.SaveChangesAsync();

        return existingEntities;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        GetDbSet().Remove(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<List<TEntity>> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        List<TEntity> entities = await GetAsync(predicate);
        GetDbSet().RemoveRange(entities);
        await _context.SaveChangesAsync();

        return entities;
    }

    public async Task<List<TEntity>> DeleteRangeAsync(List<TEntity> entities)
    {
        GetDbSet().RemoveRange(entities);
        await _context.SaveChangesAsync();

        return entities;
    }

    public IQueryable<TEntity> GetAsQueryable()
    {
        return GetDbSet();
    }

    public int GetCount(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = GetDbSet();

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
        int entityCount = GetCount(predicate);

        return entityCount > 0;
    }

    private DbSet<TEntity> GetDbSet()
    {
        return _context.Set<TEntity>();
    }

    
    /// <summary>
    /// Updates the properties of an existing entity with the values from a data object.
    /// </summary>
    /// <param name="existingEntity">The entity to be updated.</param>
    /// <param name="updateSource">The object containing the new values.</param>
    /// <remarks>
    /// This method iterates through the properties of the data object and updates the corresponding properties
    /// of the existing entity if they are not null and can be written to. If the property is a collection,
    /// it calls the <see cref="SQLGenericRepository{TEntity}.PatchCollectible"/> method to update the collection.
    /// </remarks>
    private static void Patch(object existingEntity, object updateSource)
    {
        Type entityType = existingEntity.GetType();
        Type sourceDataType = updateSource.GetType();

        foreach (PropertyInfo sourceProperty in sourceDataType.GetProperties())
        {
            object? sourcePropertyValue = sourceProperty.GetValue(updateSource);

            // Check if the source has defined a new value; if not, don't patch it
            if (sourcePropertyValue is not null)
            {
                PropertyInfo? entityProperty = entityType.GetProperty(sourceProperty.Name);

                // Ensure the existing entity has a matching property, and can be written to
                if (entityProperty is not null && entityProperty.CanWrite)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(entityProperty.PropertyType) && entityProperty.PropertyType != typeof(string))
                    {
                        // If the property is a collection, call the PatchCollectible method
                        if (entityProperty.GetValue(existingEntity) is IEnumerable<object> existingRecords
                        && sourcePropertyValue is IEnumerable<object> updatedRecords)
                        {
                            SQLGenericRepository<TEntity>.PatchCollectible(existingRecords, updatedRecords);
                        }
                    }
                    else
                    {
                        entityProperty.SetValue(existingEntity, sourcePropertyValue);
                    }
                }
            }
        }
    }

    private static void PatchCollectible(IEnumerable<object> existingCollection, IEnumerable<object> newCollection)
    {
        foreach (object newEntity in newCollection)
        {
            // Get the primary key from the existing entity
            object? primaryKey = newEntity.GetType().GetProperty("Id")?.GetValue(newEntity);

            // Check if the primary key exists in the existing collection
            object? existingItem = existingCollection.FirstOrDefault(e => 
            {
                var idValue = e.GetType().GetProperty("Id")?.GetValue(e);
                return idValue != null && idValue.Equals(primaryKey);
            });

            if (existingItem != null)
            {
                SQLGenericRepository<TEntity>.Patch(existingItem, newEntity);
            }
        }
    }
}
