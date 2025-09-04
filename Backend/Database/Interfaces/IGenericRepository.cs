using System.Linq.Expressions;

namespace Database.Interfaces;

/// <summary>
/// Defines a generic repository contract providing common CRUD operations for entity types.
/// Implements the Repository pattern with support for LINQ expressions and query customization.
/// </summary>
/// <typeparam name="TEntity">The entity type that this repository manages. Must be a reference type.</typeparam>
/// <remarks>
/// This interface provides a consistent data access layer across different entity types,
/// promoting code reuse and standardizing database interaction patterns throughout the application.
/// </remarks>
internal interface IGenericRepository<TEntity> where TEntity : class
{
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
    Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

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
    Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    /// <summary>
    /// Retrieves all entities of the specified type with optional query customization.
    /// </summary>
    /// <param name="queryModifier">Optional function to modify the query (e.g., includes, ordering).</param>
    /// <returns>A list containing all entities of the specified type.</returns>
    /// <remarks>
    /// Use with caution on large datasets. Consider using pagination or filtering for better performance.
    /// The queryModifier can be used to include related data or apply ordering.
    /// </remarks>
    Task<List<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    /// <summary>
    /// Adds a new entity to the database context for insertion.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <remarks>
    /// The entity is added to the context but not persisted until SaveChanges is called.
    /// This allows for batching multiple operations within a single transaction.
    /// </remarks>
    Task AddAsync(TEntity entity);

    /// <summary>
    /// Adds multiple entities to the database context for bulk insertion.
    /// </summary>
    /// <param name="entities">The list of entities to add.</param>
    /// <remarks>
    /// More efficient than multiple AddAsync calls for bulk operations.
    /// Entities are added to the context but not persisted until SaveChanges is called.
    /// </remarks>
    Task AddRangeAsync(List<TEntity> entities);

    /// <summary>
    /// Marks an entity for deletion from the database.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <remarks>
    /// The entity is marked for deletion but not removed until SaveChanges is called.
    /// The entity must be tracked by the context or an exception will be thrown.
    /// </remarks>
    void Delete(TEntity entity);

    /// <summary>
    /// Marks multiple entities for deletion from the database.
    /// </summary>
    /// <param name="entities">The list of entities to delete.</param>
    /// <remarks>
    /// More efficient than multiple Delete calls for bulk operations.
    /// All entities must be tracked by the context or an exception will be thrown.
    /// </remarks>
    void DeleteRange(List<TEntity> entities);

    /// <summary>
    /// Returns an IQueryable for advanced query composition and deferred execution.
    /// </summary>
    /// <returns>An IQueryable of the entity type for complex query building.</returns>
    /// <remarks>
    /// This method provides direct access to the underlying IQueryable for scenarios requiring
    /// complex queries that cannot be easily expressed through the standard repository methods.
    /// Use with caution as it breaks some repository abstraction benefits.
    /// </remarks>
    IQueryable<TEntity> GetAsQueryable();

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
    int Count(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);

    /// <summary>
    /// Checks if any entities exist matching the specified predicate.
    /// </summary>
    /// <param name="predicate">The LINQ expression defining the existence criteria.</param>
    /// <returns>True if at least one entity matches the predicate, false otherwise.</returns>
    /// <remarks>
    /// This method is optimized for existence checking and will return as soon as the first
    /// matching entity is found, making it more efficient than Count() > 0 for this purpose.
    /// </remarks>
    bool Exists(Expression<Func<TEntity, bool>> predicate);
}
