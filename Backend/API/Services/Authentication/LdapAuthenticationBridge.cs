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

/// <summary>
/// Provides LDAP authentication services implementing the IAuthenticationBridge interface.
/// This class handles LDAP connections, user authentication, and directory searches for users and groups.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><description>User authentication using simple bind with username/password</description></item>
/// <item><description>Searching for users by username with support for both userPrincipalName and sAMAccountName</description></item>
/// <item><description>Searching for groups by name</description></item>
/// <item><description>Searching for entities by GUID</description></item>
/// <item><description>Paginated user searches with role filtering</description></item>
/// <item><description>Session caching for pagination operations</description></item>
/// <item><description>Proper escaping of LDAP search filters and GUIDs</description></item>
/// </list>
/// 
/// The class automatically handles FQDN parsing and UPN conversion for authentication.
/// It maintains an internal LDAP connection and provides proper resource disposal.
/// 
/// <list type="bullet">
/// <item><description>LDAPException.ConnectionError for connection issues (LDAP result code 91)</description></item>
/// <item><description>LDAPException.InvalidCredentials for authentication failures (LDAP result code 49)</description></item>
/// <item><description>Generic LDAPException for other LDAP errors</description></item>
/// </list>
/// </remarks>
public class LdapAuthenticationBridge : IAuthenticationBridge
{
    private readonly LDAPSettings _LDAPSettings;
    private readonly JWTSettings _JWTSettings;
    private readonly LdapSessionCacheService _ldapSessionCache;
    private LdapConnection connection = new();

    public LdapAuthenticationBridge(IConfiguration configuration, LdapSessionCacheService ldapSessionCache)
    {
        _LDAPSettings = ConfigurationBinderService.Bind<LDAPSettings>(configuration);
        _JWTSettings = ConfigurationBinderService.Bind<JWTSettings>(configuration);
        LdapSearchConstraints constraints = connection.SearchConstraints;
        constraints.ReferralFollowing = true;
        connection.Constraints = constraints;
        _ldapSessionCache = ldapSessionCache;
    }

    /// <summary>
    /// Authenticates the user with the specified username and password.
    /// </summary>
    /// <param name="username">The username of the user to authenticate.</param>
    /// <param name="password">The password of the user to authenticate.</param>
    /// <exception cref="LDAPException.ConnectionError">Thrown if there is an error connecting to the LDAP server.</exception>
    /// <exception cref="LDAPException.InvalidCredentials">Thrown if the provided credentials are invalid.</exception>
    /// <exception cref="LDAPException">Thrown if an unknown LDAP error occurs.</exception>
    public void Authenticate(string username, string password)
    {
        try
        {
            if (connection.Connected)
            {
                connection.Disconnect();
            }
            connection.Connect(_LDAPSettings.Host, _LDAPSettings.Port);

            WithSimple(username, password);
        }
        catch (LdapException e)
        {
            // https://ldap.com/ldap-result-code-reference/

            while (e is not null)
            {
                // TODO: Create custom exceptions and/or look into better way of handling the various LDAP errors that may occur
                if (e.ResultCode == 91)
                {
                    throw new LDAPException.ConnectionError();
                }
                else if (e.ResultCode == 49)
                {
                    throw new LDAPException.InvalidCredentials();
                }
                else
                {
                    if (e.InnerException is LdapException ldapException)
                    {
                        e = ldapException;
                    }
                    else throw;
                }
            }

        }
    }

    public TUser? SearchUser<TUser>(string username) where TUser : new()
    {
        string domain = _LDAPSettings.FQDN.Split(".", 2).Last();

        // NETBIOS\\username
        string sAMAccountName = $"{string.Join("", domain.Split('.'))}\\\\{username}";
        // username@domain
        string userPrincipalName = $"{sAMAccountName.Split('\\').Last()}@{domain}";

        string searchFilter = $"(|(userPrincipalName={userPrincipalName})(sAMAccountName={sAMAccountName}))";

        var users = SearchLDAP<TUser>(searchFilter, _LDAPSettings.BaseDN, LdapConnection.ScopeSub);
        if (users.Count == 0)
        {
            return default;
        }
        TUser userSearch = users.First();

        return userSearch;
    }

    public TGroup? SearchGroup<TGroup>(string groupName) where TGroup : new()
    {
        string searchFilter = $"(&(objectCategory=group)(cn={EscapeLDAPSearchFilter(groupName)}))";
        var groups = SearchLDAP<TGroup>(searchFilter, _LDAPSettings.BaseDN);
        if (groups.Count == 0)
        {
            return default;
        }
        TGroup ldapGroup = groups.First();

        return ldapGroup;
    }

    public TEntity? SearchId<TEntity>(string Id) where TEntity : new()
    {
        if (string.IsNullOrEmpty(Id)) return default;

        if (!Guid.TryParse(Id, out Guid guid))
        {
            throw new ArgumentException("The provided Id is not a valid GUID.");
        }

        string searchFilter = $"(objectGUID={EscapeGUID(guid)})";
        var results = SearchLDAP<TEntity>(searchFilter, _LDAPSettings.BaseDN);
        if (results.Count == 0)
        {
            return default;
        }
        TEntity ldapObject = results.First();

        return ldapObject;
    }

    public (List<TLdapUser>, string, bool) SearchUserPagination<TLdapUser>(string username, string? userRole, int pageSize, string? sessionId) where TLdapUser : new()
    {
        byte[]? cookie;
        SessionData? sessionData = null;
        if (sessionId is not null)
        {
            sessionData = _ldapSessionCache.GetSession(sessionId);
            if (sessionData is null)
            {
                throw new HttpResponseException(HttpStatusCode.Gone, "Session ID is no longer valid. Please restart the pagination.");
            }
        }
        else
        {
            sessionId = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        if (sessionData is not null)
        {
            connection = sessionData.Connection;
            cookie = sessionData.Cookie;
        }
        else
        {
            Authenticate(_LDAPSettings.SA, _LDAPSettings.SAPassword);
            cookie = null;
        }

        LdapSearchConstraints constraints = new();
        constraints.SetControls(
        [
            new SimplePagedResultsControl(pageSize, cookie)
        ]);

        string escapedUsername = EscapeLDAPSearchFilter(username);

        string searchFilter = $"(|(userPrincipalName=*{escapedUsername}*)(sAMAccountName=*{escapedUsername}*)(name=*{escapedUsername}*))";


        if (userRole is not null)
        {
            // Converts internal role to ldap role
            var matchedRole = _JWTSettings.Roles.FirstOrDefault(x => userRole.Contains(x.Key, StringComparison.CurrentCultureIgnoreCase));
            if (matchedRole.Equals(default(KeyValuePair<string, string>)))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, $"Role mapping for '{userRole}' not found.");
            }
            userRole = matchedRole.Value;
            GroupDistinguishedName? group = SearchGroup<GroupDistinguishedName>(userRole) ?? throw new HttpResponseException(HttpStatusCode.NotFound, $"Group '{userRole}' not found.");
            searchFilter = $"(&{searchFilter}(memberOf={group.DistinguishedName.StringValue}))";
        }

        ILdapSearchResults searchResult = connection.Search(
            _LDAPSettings.BaseDN,
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

        _ldapSessionCache.StoreSession(sessionId, connection, cookie);

        return (ldapUsers, sessionId, cookie is not null && cookie.Length > 0);
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
        if (!connection.Bound) throw new LDAPException.NotBound(
            "The LDAP connection must be bound before performing a search."
            );

        string[] attributes = IAuthenticationBridge.GetEntriesToQuery<TLdapResult>();

        ILdapSearchResults searchResults = connection.Search(
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

        return mappedLdapResults;
    }

    /// <summary>
    /// Disconnects the LDAP connection and disposes of the connection object.
    /// </summary>
    /// <seealso cref="LdapConnection.Disconnect()"/>
    /// <seealso cref="LdapConnection.Dispose()"/>
    public void Dispose()
    {
        connection.Disconnect();
        connection.Dispose();
    }

    public bool IsConnected() => connection.Bound;

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
    /// Authenticates a user against the LDAP server using simple bind with the provided username and password.
    /// The username is converted to a User Principal Name (UPN) format by appending the domain extracted from the LDAP settings.
    /// </summary>
    /// <param name="username">The username of the user to authenticate.</param>
    /// <param name="password">The password of the user to authenticate.</param>
    private void WithSimple(string username, string password)
    {
        string[] fqdnParts = _LDAPSettings.FQDN.Split('.');
        if (fqdnParts.Length < 2)
        {
            throw new InvalidOperationException("FQDN must contain at least one dot and two segments.");
        }
        string domain = string.Join('.', fqdnParts.Skip(1));
        username = $"{username}@{domain}";

        // dn = The DistinguishedName of the object (user) to authenticate as.
        // UPN (User Principal Name) (username@domain),
        // and sAMAccountName (NETBIOS\\username) also works.
        connection.Bind(username, password);
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
}
