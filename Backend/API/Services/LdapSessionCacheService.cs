using Microsoft.Extensions.Caching.Memory;
using Novell.Directory.Ldap;

namespace API.Services;

public class LdapSessionCacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger _logger;
    private const int CACHEEXPIRYTIME = 10;
    private const int CACHEINACTIVEEXPIRYTIME = 5;

    public LdapSessionCacheService(IMemoryCache cache, ILoggerFactory loggerFactory)
    {
        _cache = cache;
        _logger = loggerFactory.CreateLogger(GetType());
    }


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
