
namespace API.Services;


/// <summary>
/// Provides caching functionality using IMemoryCache with configurable TTL and sliding expiration.
/// </summary>
/// <param name="memoryCache">The memory cache instance to use for storing data.</param>
/// <param name="logger">The logger instance for logging cache operations.</param>
/// <param name="ttl">The absolute time-to-live in minutes for cache entries (default: 10).</param>
/// <param name="inactiveTtl">The sliding expiration time in minutes for inactive cache entries (default: 5).</param>
public class CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger, int ttl = 10, int inactiveTtl = 5)
{
    private readonly IMemoryCache _cache = memoryCache;
    private readonly ILogger<CacheService> logger = logger;
    private readonly int CACHEEXPIRYTIME = ttl;
    private readonly int CACHEINACTIVEEXPIRYTIME = inactiveTtl;

    /// <summary>
    /// Stores data in the cache with the specified session ID.
    /// </summary>
    /// <typeparam name="T">The type of data to store.</typeparam>
    /// <param name="sessionId">The unique identifier for the cache entry.</param>
    /// <param name="data">The data to store in the cache.</param>
    /// <param name="postEvictionDelegate">Optional callback to execute when the cache entry is evicted.</param>
    /// <exception cref="ArgumentException">Thrown when sessionId is null or whitespace.</exception>
    public void Set<T>(string sessionId, T data, PostEvictionDelegate? postEvictionDelegate = null)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Session ID cannot be null or whitespace.", nameof(sessionId));
        }

        var stopwatch = Stopwatch.StartNew();
        logger.LogDebug("Storing session {SessionId} in cache", sessionId);

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(CACHEINACTIVEEXPIRYTIME),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHEEXPIRYTIME)
        };

        if (postEvictionDelegate is not null)
        {
            cacheEntryOptions.RegisterPostEvictionCallback(postEvictionDelegate);
        }

        _cache.Set(sessionId, data, cacheEntryOptions);

        stopwatch.Stop();
        logger.LogDebug("Stored session {SessionId} in cache in {ElapsedMilliseconds} ms", sessionId, stopwatch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Stores data in the cache with a generated session ID and configurable expiration settings.
    /// </summary>
    /// <typeparam name="T">The type of data to store in the cache.</typeparam>
    /// <param name="data">The data to store in the cache.</param>
    /// <param name="postEvictionDelegate">Optional callback delegate to execute when the cache entry is evicted.</param>
    /// <returns>A unique session ID (GUID) that can be used to retrieve the cached data.</returns>
    /// <remarks>
    /// The cache entry is configured with both sliding expiration and absolute expiration times.
    /// Sliding expiration resets the expiration timer when the entry is accessed.
    /// Performance metrics are logged for debugging purposes.
    /// </remarks>
    public string Set<T>(T data, PostEvictionDelegate? postEvictionDelegate = null)
    {
        var sessionId = Guid.NewGuid().ToString();

        var stopwatch = Stopwatch.StartNew();
        logger.LogDebug("Storing session {SessionId} in cache", sessionId);

        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(CACHEINACTIVEEXPIRYTIME),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHEEXPIRYTIME)
        };

        if (postEvictionDelegate is not null)
        {
            cacheEntryOptions.RegisterPostEvictionCallback(postEvictionDelegate);
        }

        _cache.Set(sessionId, data, cacheEntryOptions);

        stopwatch.Stop();
        logger.LogDebug("Stored session {SessionId} in cache in {ElapsedMilliseconds} ms", sessionId, stopwatch.ElapsedMilliseconds);

        return sessionId;
    }

    /// <summary>
    /// Retrieves data from the cache using the specified session ID.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve.</typeparam>
    /// <param name="sessionId">The unique identifier for the cache entry.</param>
    /// <returns>The cached data if found; otherwise, null.</returns>
    /// <exception cref="ArgumentException">Thrown when sessionId is null or whitespace.</exception>
    public T? Get<T>(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Session ID cannot be null or whitespace.", nameof(sessionId));
        }

        var stopwatch = Stopwatch.StartNew();
        logger.LogDebug("Retrieving session {SessionId} from cache", sessionId);

        _cache.TryGetValue(sessionId, out T? data);

        stopwatch.Stop();
        if (data is not null)
        {
            logger.LogDebug("Cache hit for session {SessionId} in {ElapsedMilliseconds} ms", sessionId, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            logger.LogDebug("Cache miss for session {SessionId} in {ElapsedMilliseconds} ms", sessionId, stopwatch.ElapsedMilliseconds);
        }

        return data;
    }

    /// <summary>
    /// Removes a cache entry with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The unique identifier for the cache entry to remove.</param>
    /// <exception cref="ArgumentException">Thrown when sessionId is null or whitespace.</exception>
    public void Remove(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Session ID cannot be null or whitespace.", nameof(sessionId));
        }

        logger.LogDebug("Removing session {SessionId} from cache", sessionId);
        _cache.Remove(sessionId);
    }
}