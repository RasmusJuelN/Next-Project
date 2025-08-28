using Microsoft.Extensions.Caching.Memory;
using Novell.Directory.Ldap;

namespace API.Services;

/// <summary>
/// Manages in-memory caching of LDAP connection sessions to optimize performance and resource utilization.
/// This service handles the lifecycle of LDAP connections including storage, retrieval, and automatic cleanup
/// to prevent connection leaks and ensure efficient session management for paginated LDAP queries.
/// </summary>
/// <remarks>
/// The service implements a time-based cache eviction strategy with both sliding and absolute expiration:
/// <list type="bullet">
/// <item><description>Sliding expiration: Resets when session is accessed (5 minutes default)</description></item>
/// <item><description>Absolute expiration: Maximum session lifetime (10 minutes default)</description></item>
/// <item><description>Automatic cleanup: Disconnects and disposes LDAP connections on eviction</description></item>
/// </list>
/// This ensures that LDAP connections are properly managed and don't accumulate over time.
/// </remarks>
public class LdapSessionCacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private const int CACHEEXPIRYTIME = 10;
    private const int CACHEINACTIVEEXPIRYTIME = 5;

    /// <summary>
    /// Initializes a new instance of the <see cref="LdapSessionCacheService"/> class.
    /// </summary>
    /// <param name="cache">The memory cache instance for storing session data.</param>
    /// <param name="loggerFactory">The logger factory for creating service-specific loggers.</param>
    /// <remarks>
    /// The service uses dependency injection to receive the memory cache and logger factory,
    /// ensuring proper integration with the application's caching and logging infrastructure.
    /// </remarks>
    public LdapSessionCacheService(IMemoryCache cache, ILoggerFactory loggerFactory)
    {
        _cache = cache;
        _logger = loggerFactory.CreateLogger(GetType());
    }

    /// <summary>
    /// Stores an LDAP connection session in the cache with automatic expiration and cleanup handling.
    /// </summary>
    /// <param name="sessionId">The unique identifier for the session.</param>
    /// <param name="ldapConnection">The active LDAP connection to cache.</param>
    /// <param name="sessionCookie">Optional session cookie for pagination continuation.</param>
    /// <remarks>
    /// This method configures cache entries with:
    /// <list type="bullet">
    /// <item><description>Sliding expiration of 5 minutes (resets on access)</description></item>
    /// <item><description>Absolute expiration of 10 minutes (maximum lifetime)</description></item>
    /// <item><description>Post-eviction callback for automatic connection cleanup</description></item>
    /// </list>
    /// The callback ensures that LDAP connections are properly disconnected and disposed
    /// when the cache entry expires, preventing connection leaks.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when sessionId or ldapConnection is null.</exception>
    /// <exception cref="ArgumentException">Thrown when sessionId is empty or whitespace.</exception>
    public void StoreSession(string sessionId, LdapConnection ldapConnection, byte[]? sessionCookie)
    {
        SessionData sessionData = new()
        {
            Connection = ldapConnection,
            Cookie = sessionCookie
        };

        MemoryCacheEntryOptions cacheOptions = new()
        {
            SlidingExpiration = TimeSpan.FromMinutes(CACHEINACTIVEEXPIRYTIME), // Set inactive lifetime
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHEEXPIRYTIME)
        };

        cacheOptions.RegisterPostEvictionCallback((key, value, reason, state) =>
        {
            if (state is LdapConnection connection)
            {
                _logger.LogDebug("Disconnecting session on evicted cache with ID {sessionId}", sessionId);
                connection.Disconnect();
                connection.Dispose();
            }
        });

        _cache.Set(sessionId, sessionData, cacheOptions);
    }

    public SessionData? GetSession(string sessionId)
    {
        _cache.TryGetValue(sessionId, out SessionData? sessionData);

        if (sessionData is not null)
        {
            _logger.LogDebug("Cache hit on ID {sessionId}", sessionId);
        }
        else
        {
            _logger.LogDebug("Cache miss on ID {sessionId}", sessionId);
        }
        
        return sessionData;
    }

    public void RemoveSession(string sessionId)
    {
        _logger.LogDebug("Evicting cache with ID {sessionId}", sessionId);
        _cache.Remove(sessionId);
    }
}

public class SessionData
{
    public required LdapConnection Connection { get; set; }
    public byte[]? Cookie { get; set; }
}
