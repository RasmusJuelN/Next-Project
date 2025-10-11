using System.Net;
using System.Reflection;
using API.Attributes;
using API.DTO.LDAP;
using API.Exceptions;
using API.Interfaces;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Controls;
using Settings.Models;

namespace API.Services.Authentication;

public class ActiveDirectoryAuthenticationBridge(
    ILogger<ActiveDirectoryAuthenticationBridge> logger,
    IConfiguration configuration,
    CacheService sessionCacheService,
    LdapConnection? ldapConnection = null,
    string? sessionId = null) : IAuthenticationBridge
{
    private readonly ILogger<ActiveDirectoryAuthenticationBridge> _Logger = logger;
    private readonly LDAPSettings _LdapSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
    private readonly JWTSettings _JwtSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
    private readonly CacheService _cache = sessionCacheService;
    private LdapConnection? _Connection = ldapConnection;
    private string? _SessionId = sessionId;

    public void Authenticate(string username, string password)
    {
        _Logger.LogInformation("Starting LDAP authentication for user: {Username}", username);

        int port;
        LdapConnectionOptions connectionOptions = new();
        if (_LdapSettings.UseSSL)
        {
            connectionOptions.UseSsl();
            port = _LdapSettings.SSLPort;
        }
        else
        {
            port = _LdapSettings.Port;
        }

        try
        {
            if (_Connection is not null && _Connection.Connected)
            {
                _Logger.LogDebug("Disconnecting existing LDAP connection");
                _Connection.Disconnect();
            }

            if (!string.IsNullOrWhiteSpace(_SessionId))
            {
                _Logger.LogDebug("Using cached session connection for session: {SessionId}", _SessionId);
                _Connection = GetCachedSessionConnection(_SessionId);
                _SessionId = null;
            }
            else
            {
                _Logger.LogDebug("Creating new LDAP connection to {Host}:{Port}", _LdapSettings.Host, _LdapSettings.Port);
                _Connection = CreateConnection(connectionOptions);
                LdapSearchConstraints constraints = new();
                constraints.ReferralFollowing = true;
                _Connection.Constraints = constraints;
            }

            _Connection.Connect(_LdapSettings.Host, port);

            BindWithSimple(username, password);
            _Logger.LogInformation("LDAP authentication successful for user: {Username}", username);

        }
        catch (LdapException ex)
        {
            _Logger.LogError(ex, "LDAP authentication failed for user: {Username} with result code: {ResultCode}", username, ex.ResultCode);

            // https://ldap.com/ldap-result-code-reference/
            throw ex.ResultCode switch
            {
                LdapException.ConnectError => new UnauthorizedAccessException("Unable to connect to the LDAP server.", ex),
                LdapException.InvalidCredentials => new UnauthorizedAccessException("Invalid username or password.", ex),
                LdapException.LdapTimeout => new InvalidOperationException("The LDAP server did not respond in a timely manner.", ex),
                LdapException.ServerDown => new InvalidOperationException("The LDAP server is currently unreachable.", ex),
                _ => new InvalidOperationException($"LDAP authentication failed with the following result code and message: {ex.ResultCode} - {ex.Message}", ex),
            };
        }
    }

    public TUser? SearchUser<TUser>(string username) where TUser : new()
    {
        _Logger.LogDebug("Starting user search for: {Username}", username);
        
        string domain = _LdapSettings.FQDN.Split(".", 2).Last();

        // NETBIOS\\username
        string sAMAccountName = $"{string.Join("", domain.Split('.'))}\\\\{username}";
        // username@domain
        string userPrincipalName = $"{sAMAccountName.Split('\\').Last()}@{domain}";

        string searchFilter = $"(|(userPrincipalName={userPrincipalName})(sAMAccountName={sAMAccountName}))";
        _Logger.LogDebug("Using search filter: {SearchFilter} with BaseDN: {BaseDN}", searchFilter, _LdapSettings.BaseDN);

        var users = SearchLDAP<TUser>(searchFilter, _LdapSettings.BaseDN, LdapConnection.ScopeSub);
        if (users.Count == 0)
        {
            _Logger.LogWarning("No users found for username: {Username}", username);
            return default;
        }
        
        _Logger.LogDebug("Found {UserCount} user(s) for username: {Username}", users.Count, username);
        TUser userSearch = users.First();

        return userSearch;
    }

    public TGroup? SearchGroup<TGroup>(string groupName) where TGroup : new()
    {
        _Logger.LogDebug("Starting group search for: {GroupName}", groupName);
        
        string escapedGroupName = EscapeLDAPSearchFilter(groupName);
        TGroup? cachedLdapGroup = _cache.Get<TGroup>(escapedGroupName);

        if (cachedLdapGroup is null)
        {
            string searchFilter = $"(&(objectCategory=group)(cn={EscapeLDAPSearchFilter(groupName)}))";
            _Logger.LogDebug("Using group search filter: {SearchFilter}", searchFilter);

            var groups = SearchLDAP<TGroup>(searchFilter, _LdapSettings.BaseDN);
            if (groups.Count == 0)
            {
                _Logger.LogWarning("No groups found for group name: {GroupName}", groupName);
                return default;
            }

            _Logger.LogDebug("Found {GroupCount} group(s) for group name: {GroupName}", groups.Count, groupName);
            TGroup ldapGroup = groups.First();

            _cache.Set(escapedGroupName, ldapGroup);

            return ldapGroup;
        }
        else
        {
            _Logger.LogDebug("Found cached group for group name: {GroupName}", groupName);
            return cachedLdapGroup;
        }
    }

    public TEntity? SearchId<TEntity>(string Id) where TEntity : new()
    {
        _Logger.LogDebug("Starting entity search by ID: {Id}", Id);
        
        if (string.IsNullOrEmpty(Id))
        {
            _Logger.LogWarning("Provided ID is null or empty");
            return default;
        }

        if (!Guid.TryParse(Id, out Guid guid))
        {
            _Logger.LogError("The provided Id is not a valid GUID: {Id}", Id);
            throw new ArgumentException("The provided Id is not a valid GUID.");
        }

        string searchFilter = $"(objectGUID={EscapeGUID(guid)})";
        _Logger.LogDebug("Using ID search filter: {SearchFilter}", searchFilter);
        
        var results = SearchLDAP<TEntity>(searchFilter, _LdapSettings.BaseDN);
        if (results.Count == 0)
        {
            _Logger.LogWarning("No entities found for ID: {Id}", Id);
            return default;
        }
        
        _Logger.LogDebug("Found {ResultCount} entity(ies) for ID: {Id}", results.Count, Id);
        TEntity ldapObject = results.First();

        return ldapObject;
    }

    public (List<TLdapUser>, string, bool) SearchUserPagination<TLdapUser>(string username, string? userRole, int pageSize, string? sessionId) where TLdapUser : new()
    {
        _Logger.LogInformation("Starting paginated user search - Username: {Username}, UserRole: {UserRole}, PageSize: {PageSize}, SessionId: {SessionId}", 
            username, userRole, pageSize, sessionId);
            
        byte[]? cookie;
        SessionData? sessionData = null;
        if (sessionId is not null)
        {
            _Logger.LogDebug("Retrieving existing session data for ID: {SessionId}", sessionId);
            sessionData = _cache.Get<SessionData>(sessionId);
            if (sessionData is null)
            {
                _Logger.LogWarning("Session ID is no longer valid: {SessionId}", sessionId);
                throw new HttpResponseException(HttpStatusCode.Gone, "Session ID is no longer valid. Please restart the pagination.");
            }
        }
        else
        {
            sessionId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _Logger.LogDebug("Generated new session ID: {SessionId}", sessionId);
        }

        if (sessionData is not null)
        {
            _Logger.LogDebug("Using existing session connection for pagination");
            _Connection = sessionData.Connection;
            cookie = sessionData.Cookie;
        }
        else
        {
            _Logger.LogDebug("Creating new service account connection for pagination");
            Authenticate(_LdapSettings.SA, _LdapSettings.SAPassword);
            cookie = null;
        }

        LdapSearchConstraints constraints = new();
        constraints.SetControls(
        [
            new SimplePagedResultsControl(pageSize, cookie)
        ]);

        string escapedUsername = EscapeLDAPSearchFilter(username);

        string searchFilter = $"(|(userPrincipalName=*{escapedUsername}*)(sAMAccountName=*{escapedUsername}*)(name=*{escapedUsername}*))";
        _Logger.LogDebug("Initial search filter: {SearchFilter}", searchFilter);

        if (userRole is not null)
        {
            _Logger.LogDebug("Applying role filter for: {UserRole}", userRole);
            // Converts internal role to ldap role
            KeyValuePair<string, string>? matchedRole = _JwtSettings.Roles.FirstOrDefault(x => userRole.Contains(x.Key, StringComparison.CurrentCultureIgnoreCase));
            if (matchedRole is null || matchedRole.Equals(default(KeyValuePair<string, string>)))
            {
                _Logger.LogError("Role mapping not found for: {UserRole}", userRole);
                throw new HttpResponseException(HttpStatusCode.NotFound, $"Role mapping for '{userRole}' not found.");
            }
            userRole = matchedRole.Value.Value;
            _Logger.LogDebug("Mapped internal role to LDAP role: {LdapRole}", userRole);
            
            GroupDistinguishedName? group = SearchGroup<GroupDistinguishedName>(userRole) ?? throw new HttpResponseException(HttpStatusCode.NotFound, $"Group '{userRole}' not found.");
            searchFilter = $"(&{searchFilter}(memberOf={group.DistinguishedName.StringValue}))";
            _Logger.LogDebug("Final search filter with role: {SearchFilter}", searchFilter);
        }

        ILdapSearchResults searchResult = _Connection!.Search(
            _LdapSettings.BaseDN,
            LdapConnection.ScopeSub,
            searchFilter,
            IAuthenticationBridge.GetEntriesToQuery<TLdapUser>(),
            false,
            constraints
        );

        List<TLdapUser> ldapUsers = [];

        while (searchResult.HasMore())
        {
            LdapEntry entry = searchResult.Next();
            ldapUsers.Add(MapEntry<TLdapUser>(entry));
        }

        foreach (LdapControl control in searchResult?.ResponseControls ?? [])
        {
            if (control is SimplePagedResultsControl response)
            {
                cookie = response.Cookie;
            }
        }

        _Logger.LogDebug("Found {UserCount} users in pagination search", ldapUsers.Count);
        _cache.Set(sessionId, new SessionData{ Connection = _Connection, Cookie = cookie });
        
        bool hasMoreResults = cookie is not null && cookie.Length > 0;
        _Logger.LogInformation("Completed paginated user search - Returned {UserCount} users, HasMore: {HasMore}, SessionId: {SessionId}", 
            ldapUsers.Count, hasMoreResults, sessionId);

        return (ldapUsers, sessionId, hasMoreResults);
    }

    /// <summary>
    /// Searches the LDAP directory using the specified search query and maps the results to a list of objects of type <typeparamref name="TLdapResult"/>.
    /// <para></para>
    /// IMPORTANT: Please ensure any special characters that may need to be used in the query itself are properly escaped.
    /// They can either be escaped manually with <c>\\</c>, or by running the value you're searching for through <see cref="EscapeLDAPSearchFilter"/>
    /// <para></para>
    /// LDAP supports AND (<c>&amp;</c>), OR (<c>|</c>), Wildcards (<c>*</c>), Equality (<c>=</c>), 
    /// Negation (<c>!</c>), and Greater/Less than (<c>&gt;</c>|<c>&lt;</c>).
    /// <para></para>
    /// Examples:
    /// <br></br>
    /// <c>(|(userPrincipalName=Nick)(sAMAccountName=Nick))</c> = userPrincipalName OR sAMAccountName is Nick.
    /// <br></br>
    /// <c>(!(userPrincipalName=Nick))</c> = userPrincipalName is NOT Nick.
    /// <br></br>
    /// <c>(userPrincipalName=*)</c> = Any object which has a value in userPrincipalName. Please don't actually do that :(
    /// <br></br>
    /// <c>(userPrincipalName=*Nick*)</c> = Any object where "Nick" is in the value in userPrincipalName.
    /// <para></para>
    /// https://www.ldapexplorer.com/en/manual/109010000-ldap-filter-syntax.htm can be refered to for more insight.
    /// </summary>
    /// <typeparam name="TLdapResult">The type of objects to map the LDAP search results to. Must have a parameterless constructor.</typeparam>
    /// <param name="searchQuery">The LDAP search query.</param>
    /// <param name="baseDN">The base distinguished name (DN) to search from.</param>
    /// <param name="scope">The scope of the search. Defaults to <see cref="LdapConnection.ScopeSub"/>.</param>
    /// <param name="attributes">The attributes to retrieve. Defaults to all attributes if not specified.</param>
    /// <returns>A list of objects of type <typeparamref name="TLdapResult"/> mapped from the LDAP search results.</returns>
    /// <exception cref="LDAPException.NotBound">Thrown if the LDAP connection is not bound.</exception>
    private List<TLdapResult> SearchLDAP<TLdapResult>(string searchQuery, string baseDN, int scope = LdapConnection.ScopeSub) where TLdapResult : new()
    {
        _Logger.LogDebug("Executing LDAP search - Query: {SearchQuery}, BaseDN: {BaseDN}, Scope: {Scope}", searchQuery, baseDN, scope);
        
        EnsureBoundConnection();

        string[] attributes = IAuthenticationBridge.GetEntriesToQuery<TLdapResult>();
        _Logger.LogDebug("Requesting attributes: {Attributes}", string.Join(", ", attributes));

        ILdapSearchResults searchResults = _Connection!.Search(
            baseDN,
            scope,
            searchQuery,
            attributes,
            false
        );

        List<TLdapResult> mappedLdapResults = [];

        while (searchResults.HasMore())
        {
            LdapEntry entry = searchResults.Next();

            mappedLdapResults.Add(MapEntry<TLdapResult>(entry));
        }

        _Logger.LogDebug("LDAP search completed - Found {ResultCount} entries", mappedLdapResults.Count);
        return mappedLdapResults;
    }

    /// <summary>
    /// Disconnects the LDAP connection and disposes of the connection object.
    /// </summary>
    /// <seealso cref="LdapConnection.Disconnect()"/>
    /// <seealso cref="LdapConnection.Dispose()"/>
    public void Dispose()
    {
        _Connection?.Disconnect();
        _Connection?.Dispose();
    }

    public bool IsConnected() => _Connection is not null && _Connection.Bound;

    /// <summary>
    /// Escapes special characters in an LDAP search filter.
    /// </summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped value.</returns>
    public static string EscapeLDAPSearchFilter(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        // RFC4515: (, ), *, \, and NUL must be escaped as \28, \29, \2a, \5c, \00 respectively
        var escaped = new System.Text.StringBuilder();
        foreach (char c in value)
        {
            switch (c)
            {
                case '\\':
                    escaped.Append(@"\5c");
                    break;
                case '*':
                    escaped.Append(@"\2a");
                    break;
                case '(':
                    escaped.Append(@"\28");
                    break;
                case ')':
                    escaped.Append(@"\29");
                    break;
                case '\0':
                    escaped.Append(@"\00");
                    break;
                default:
                    escaped.Append(c);
                    break;
            }
        }
        return escaped.ToString();
    }

    /// <summary>
    /// Escapes a GUID for use in an LDAP search filter.
    /// </summary>
    /// <param name="guid">The GUID to escape.</param>
    /// <returns>The escaped GUID as a string.</returns>
    public static string EscapeGUID(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        return string.Concat(bytes.Select(b => $"\\{b:X2}"));
    }

    /// <summary>
    /// Ensures that the LDAP connection is established and bound.
    /// If the connection is null or not bound, it will attempt to bind using the service account credentials.
    /// </summary>
    /// <remarks>
    /// This method checks the current state of the LDAP connection and performs a simple bind
    /// operation with the configured service account if necessary to maintain connectivity.
    /// </remarks>
    private void EnsureBoundConnection()
    {
        if (_Connection is null || !_Connection.Bound)
        {
            _Logger.LogDebug("Connection is null or not bound, establishing service account connection");
            BindWithSimple(_LdapSettings.SA, _LdapSettings.SAPassword);
        }
    }

    /// <summary>
    /// Maps the properties of a <typeparamref name="MappedEntity"/> to the corresponding values from the provided LDAP entry.
    /// </summary>
    /// <typeparam name="MappedEntity">
    /// The type of the entity to map to. Must have a parameterless constructor.
    /// </typeparam>
    /// <param name="entry">
    /// The LDAP entry object to map from. Must be of type <c>LdapEntry</c>.
    /// </param>
    /// <returns>
    /// An instance of <typeparamref name="MappedEntity"/> with properties set from the LDAP entry attributes.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided <paramref name="entry"/> is not of type <c>LdapEntry</c>.
    /// </exception>
    public MappedEntity MapEntry<MappedEntity>(object entry) where MappedEntity : new()
    {
        if (entry is not LdapEntry ldapEntry)
        {
            throw new ArgumentException("The provided entry must be of type LdapEntry.");
        }

        MappedEntity mappedEntity = new();

        foreach (PropertyInfo prop in typeof(MappedEntity).GetProperties())
        {
            foreach (AuthenticationMapping attr in prop.GetCustomAttributes<AuthenticationMapping>())
            {
                prop.SetValue(mappedEntity, ldapEntry.GetAttribute(attr.EntryName));
            }
        }

        return mappedEntity;
    }

    /// <summary>
    /// Authenticates a user against the LDAP server using simple bind with the provided username and password.
    /// The username is converted to a User Principal Name (UPN) format by appending the domain extracted from the LDAP settings.
    /// </summary>
    /// <param name="username">The username of the user to authenticate.</param>
    /// <param name="password">The password of the user to authenticate.</param>
    private void BindWithSimple(string username, string password)
    {
        _Logger.LogDebug("Attempting simple bind for user: {Username}", username);
        
        string[] fqdnParts = _LdapSettings.FQDN.Split('.');
        if (fqdnParts.Length < 2)
        {
            _Logger.LogError("Invalid FQDN format: {FQDN}", _LdapSettings.FQDN);
            throw new InvalidOperationException("FQDN must contain at least one dot and two segments.");
        }
        string domain = string.Join('.', fqdnParts.Skip(1));
        username = $"{username}@{domain}";
        _Logger.LogDebug("Using UPN format for authentication: {UPN}", username);

        // dn = The DistinguishedName of the object (user) to authenticate as.
        // UPN (User Principal Name) (username@domain),
        // and sAMAccountName (NETBIOS\\username) also works.
        _Connection = GetConnection();
        
        try
        {
            _Connection.Bind(username, password);
            _Logger.LogDebug("Simple bind successful for user: {Username}", username);
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Simple bind failed for user: {Username}", username);
            throw;
        }
    }

    /// <summary>
    /// Gets an existing LDAP connection or creates a new one if none exists.
    /// </summary>
    /// <param name="options">Optional connection options to configure the LDAP connection. If null, default options will be used.</param>
    /// <returns>An <see cref="LdapConnection"/> instance that can be used for LDAP operations.</returns>
    /// <remarks>
    /// This method implements a lazy initialization pattern where the connection is only created when needed.
    /// If a connection already exists (_Connection is not null), it will be returned directly.
    /// Otherwise, a new connection will be created using the CreateConnection() method.
    /// </remarks>
    private LdapConnection GetConnection(LdapConnectionOptions? options = null)
    {
        if (_Connection is null)
        {
            _Logger.LogDebug("Creating new LDAP connection");
        }
        return _Connection ?? CreateConnection();
    }

    /// <summary>
    /// Creates a new LDAP connection instance with optional configuration.
    /// </summary>
    /// <param name="options">Optional LDAP connection configuration. If null, creates a connection with default settings.</param>
    /// <returns>A new <see cref="LdapConnection"/> instance configured with the specified options or default settings.</returns>
    private static LdapConnection CreateConnection(LdapConnectionOptions? options = null)
    {
        return options is null ? new LdapConnection() : new LdapConnection(options);
    }

    /// <summary>
    /// Retrieves a cached LDAP connection associated with the specified session ID.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the session for which to retrieve the LDAP connection.</param>
    /// <returns>The <see cref="LdapConnection"/> object associated with the session.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no cached session is found for the provided session ID.</exception>
    private LdapConnection GetCachedSessionConnection(string sessionId)
    {
        _Logger.LogDebug("Retrieving cached session connection for session: {SessionId}", sessionId);
        SessionData? data = _cache.Get<SessionData>(sessionId) ?? throw new InvalidOperationException("No cached session found for the provided session ID.");
        return data.Connection;
    }
}
