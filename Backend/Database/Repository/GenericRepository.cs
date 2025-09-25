using System.Linq.Expressions;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

/// <summary>
/// Implements a generic repository providing common CRUD operations for entity types.
/// Implements the Repository pattern with support for LINQ expressions, query customization, and comprehensive logging.
/// </summary>
/// <typeparam name="TEntity">The entity type that this repository manages. Must be a reference type.</typeparam>
/// <remarks>
/// This implementation provides a consistent data access layer across different entity types,
/// promoting code reuse and standardizing database interaction patterns. Includes logging infrastructure
/// and query optimization features while maintaining the abstraction benefits of the repository pattern.
/// </remarks>
internal class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    // TODO: Create custom exceptions and include logging
    private readonly Context _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the GenericRepository with the specified context and logger factory.
    /// </summary>
    /// <param name="context">The database context for entity operations.</param>
    /// <param name="loggerFactory">Factory for creating loggers for diagnostic and monitoring purposes.</param>
    /// <remarks>
    /// Sets up the DbSet for the entity type and creates a logger for tracking repository operations.
    /// The logger is configured with the repository's type information for contextual logging.
    /// </remarks>
    public GenericRepository(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
        _logger = loggerFactory.CreateLogger(GetType());
    }

    /// <summary>
    /// Retrieves a list of entities matching the specified predicate with optional query customization.
    /// </summary>
    /// <param name="predicate">The LINQ expression defining the filter criteria.</param>
    /// <param name="queryModifier">Optional function to modify the query (e.g., includes, ordering).</param>
    /// <returns>A list of entities matching the criteria.</returns>
    /// <remarks>
    /// The queryModifier parameter allows for complex query operations like eager loading,
    /// ordering, and additional filtering without breaking the repository abstraction.
    /// </remarks>
    public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Retrieves a single entity matching the specified predicate with optional query customization.
    /// </summary>
    /// <param name="predicate">The LINQ expression defining the filter criteria.</param>
    /// <param name="queryModifier">Optional function to modify the query (e.g., includes).</param>
    /// <returns>The first entity matching the criteria, or null if none found.</returns>
    /// <exception cref="InvalidOperationException">Thrown when multiple entities match the predicate.</exception>
    /// <remarks>
    /// This method uses SingleOrDefault semantics - it will throw an exception if multiple entities match.
    /// Use GetAsync if multiple results are expected.
    /// </remarks>
    public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _dbSet.Where(predicate);

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }

        return await query.SingleOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves all entities of the specified type with optional query customization.
    /// </summary>
    /// <param name="queryModifier">Optional function to modify the query (e.g., includes, ordering).</param>
    /// <returns>A list containing all entities of the specified type.</returns>
    /// <remarks>
    /// Use with caution on large datasets. Consider using pagination or filtering for better performance.
    /// The queryModifier can be used to include related data or apply ordering.
    /// </remarks>
    public async Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (queryModifier is not null)
        {
            query = queryModifier(query);
        }
        
        return await query.ToListAsync();
    }

    /// <summary>
    /// Adds a new entity to the database context for insertion.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <remarks>
    /// The entity is added to the context but not persisted until SaveChanges is called.
    /// This allows for batching multiple operations within a single transaction.
    /// </remarks>
    public async Task AddAsync(TEntity entity) => await _dbSet.AddAsync(entity);

    /// <summary>
    /// Adds multiple entities to the database context for bulk insertion.
    /// </summary>
    /// <param name="entities">The list of entities to add.</param>
    /// <remarks>
    /// More efficient than multiple AddAsync calls for bulk operations.
    /// Entities are added to the context but not persisted until SaveChanges is called.
    /// </remarks>
    public async Task AddRangeAsync(List<TEntity> entities) => await _dbSet.AddRangeAsync(entities);

    /// <summary>
    /// Marks an entity for deletion from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <remarks>
    /// The entity is marked for deletion but not removed until SaveChanges is called.
    /// The entity must be tracked by the context or an exception will be thrown.
    /// </remarks>
    public void Delete(TEntity entity) => _dbSet.Remove(entity);

    /// <summary>
    /// Marks multiple entities for deletion from the database.
    /// </summary>
    /// <param name="entities">The list of entities to delete.</param>
    /// <remarks>
    /// More efficient than multiple Delete calls for bulk operations.
    /// All entities must be tracked by the context or an exception will be thrown.
    /// </remarks>
    public void DeleteRange(List<TEntity> entities) => _dbSet.RemoveRange(entities);

    /// <summary>
    /// Returns an IQueryable for advanced query composition and deferred execution.
    /// </summary>
    /// <returns>An IQueryable of the entity type for complex query building.</returns>
    /// <remarks>
    /// This method provides direct access to the underlying IQueryable for scenarios requiring
    /// complex queries that cannot be easily expressed through the standard repository methods.
    /// Use with caution as it breaks some repository abstraction benefits.
    /// </remarks>
    public IQueryable<TEntity> GetAsQueryable() => _dbSet;

    /// <summary>
    /// Counts the number of entities matching the specified criteria.
    /// </summary>
    /// <param name="predicate">Optional LINQ expression defining the filter criteria. If null, counts all entities.</param>
    /// <param name="queryModifier">Optional function to modify the query before counting.</param>
    /// <returns>The number of entities matching the criteria.</returns>
    /// <remarks>
    /// This method executes a COUNT query at the database level for optimal performance.
    /// Avoid loading entities into memory when only a count is needed.
    /// </remarks>
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

    /// <summary>
    /// Checks if any entities exist matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The LINQ expression defining the existence criteria.</param>
    /// <returns>True if at least one entity matches the predicate, false otherwise.</returns>
    /// <remarks>
    /// This method is optimized for existence checking and will return as soon as the first
    /// matching entity is found, making it more efficient than Count() > 0 for this purpose.
    /// </remarks>
    public bool Exists(Expression<Func<TEntity, bool>> predicate)
    {
        int entityCount = Count(predicate);

        return entityCount > 0;
    }
}
