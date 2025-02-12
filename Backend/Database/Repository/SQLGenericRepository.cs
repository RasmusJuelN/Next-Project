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
        if (!ObjectMatchesEntity(newValues))
        {
            throw new Exception("Data object does not match entity type.");
        }

        Patch(existingEntity, newValues);
        await _context.SaveChangesAsync();

        return existingEntity;
    }

    public async Task<List<TEntity>> PatchAsync(Expression<Func<TEntity, bool>> predicate, object newValues)
    {
        if (!ObjectMatchesEntity(newValues))
        {
            throw new Exception("Data object does not match entity type.");
        }

        List<TEntity> existingEntities = await GetAsync(predicate);

        foreach (TEntity existingEntity in existingEntities)
        {
            Patch(existingEntity, newValues);
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

    private bool ObjectMatchesEntity(object dataObject)
    {
        Type entityType = typeof(TEntity);
        Type dataObjectType = dataObject.GetType();
        
        foreach (PropertyInfo dataProperty in dataObjectType.GetProperties())
        {
            PropertyInfo? entityProperty = entityType.GetProperty(dataProperty.Name);
            
            if (entityProperty is null)
            {
                _logger.LogWarning("Property {dataPropertyName} not found in entity type", dataProperty.Name);
                return false;
            }

            if (entityProperty.PropertyType != dataProperty.PropertyType)
            {
                _logger.LogWarning("Property {dataPropertyName} with type {dataPropertyDeclaringType} does not match entity property type {entityPropertyClrType}", dataProperty.Name, dataProperty.PropertyType, entityProperty.PropertyType);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Updates the properties of an existing entity with the values from a data object.
    /// Only non-null properties from the data object will be copied to the existing entity.
    /// </summary>
    /// <param name="existingEntity">The entity to be updated.</param>
    /// <param name="dataObject">The object containing the new values.</param>
    /// <typeparam name="TEntity">The type of the entity being updated.</typeparam>
    private static void Patch(TEntity existingEntity, object dataObject)
    {
        Type entityType = typeof(TEntity);
        Type dataObjectType = dataObject.GetType();

        foreach (PropertyInfo dataObjectProperty in dataObjectType.GetProperties())
        {
            object? dataObjectValue = dataObjectProperty.GetValue(dataObject);

            if (dataObjectValue is not null)
            {
                PropertyInfo? entityProperty = entityType.GetProperty(dataObjectProperty.Name);

                if (entityProperty is not null && entityProperty.CanWrite)
                {
                    entityProperty.SetValue(existingEntity, dataObjectValue);
                }
            }
        }
    }
}
