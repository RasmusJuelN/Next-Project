using Settings.Models;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Sasl;
using API.Exceptions;
using System.Reflection;
using API.Attributes;
using Novell.Directory.Ldap.Controls;
using API.DTO.LDAP;
using System.Net;
using System.Collections;

namespace API.Services;

public class LdapService
{
    private readonly LDAPSettings _LDAPSettings;
    private readonly JWTSettings _JWTSettings;
    private readonly LdapSessionCacheService _ldapSessionCache;
    public LdapConnection connection = new();

    public LdapService(IConfiguration configuration, LdapSessionCacheService ldapSessionCache)
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
            connection.Connect(_LDAPSettings.Host, _LDAPSettings.Port);

            if (connection.IsSaslMechanismSupported(SaslConstants.Mechanism.DigestMd5))
            {
                WithSASL(username, password);
            }
            else WithSimple(username, password);
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

    /// <summary>
    /// Authenticates using the service account and password specified in the configuration file
    /// </summary>
    /// <param name="username">The username of the user to authenticate.</param>
    /// <param name="password">The password of the user to authenticate.</param>
    /// <exception cref="LDAPException.ConnectionError">Thrown if there is an error connecting to the LDAP server.</exception>
    /// <exception cref="LDAPException.InvalidCredentials">Thrown if the provided credentials are invalid.</exception>
    /// <exception cref="LDAPException">Thrown if an unknown LDAP error occurs.</exception>
    public void Authenticate()
    {
        string username = _LDAPSettings.SA;
        string password = _LDAPSettings.SAPassword;

        try
        {
            connection.Connect(_LDAPSettings.Host, _LDAPSettings.Port);

            if (connection.IsSaslMechanismSupported(SaslConstants.Mechanism.DigestMd5))
            {
                WithSASL(username, password);
            }
            else WithSimple(username, password);
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
    public List<TLdapResult> SearchLDAP<TLdapResult>(string searchQuery, string baseDN, int scope = LdapConnection.ScopeSub) where TLdapResult : new()
    {
        if (!connection.Bound) throw new LDAPException.NotBound(
            "A bound connection is required. Please first use the authorization method."
            );
        
        string[] attributes = GetQueryAttributes<TLdapResult>();

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

            mappedLdapResults.Add(MapLdapEntry<TLdapResult>(entry));
        }
        
        return mappedLdapResults;
    }

    /// <summary>
    /// Searches the LDAP directory for a user with the specified username and maps the results to an object of type <typeparamref name="TLdapUser"/>.
    /// </summary>
    /// <typeparam name="TLdapUser">The type of object to map the LDAP search results to. Must have a parameterless constructor.</typeparam>
    /// <returns>An object of type <typeparamref name="TLdapUser"/> mapped from the LDAP search results.</returns>
    /// <exception cref="LDAPException.NotBound">Thrown if the LDAP connection is not bound.</exception>
    public TLdapUser SearchUser<TLdapUser>(string username) where TLdapUser : new()
    {
        string domain = _LDAPSettings.FQDN.Split(".", 2).Last();
        
        // NETBIOS\\username
        string sAMAccountName = $"{string.Join("", domain.Split('.'))}\\\\{username}";
        // username@domain
        string userPrincipalName = $"{sAMAccountName.Split('\\').Last()}@{domain}";
        
        string searchFilter = $"(|(userPrincipalName={userPrincipalName})(sAMAccountName={sAMAccountName}))";

        TLdapUser userSearch = SearchLDAP<TLdapUser>(searchFilter, _LDAPSettings.BaseDN, LdapConnection.ScopeSub).First();

        return userSearch;
    }

    public TLdapObject SearchByObjectGUID<TLdapObject>(Guid id) where TLdapObject : new()
    {
        string searchFilter = $"(objectGUID={EscapeGUID(id)})";
        TLdapObject ldapObject = SearchLDAP<TLdapObject>(searchFilter, _LDAPSettings.BaseDN).First();

        return ldapObject;
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

    public TLdapGroup SearchGroup<TLdapGroup>(string groupName) where TLdapGroup : new()
    {
        string searchFilter = $"(&(objectCategory=group)(cn={EscapeLDAPSearchFilter(groupName)}))";
        TLdapGroup ldapGroup = SearchLDAP<TLdapGroup>(searchFilter, _LDAPSettings.BaseDN).First();

        return ldapGroup;
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
            Authenticate();
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
            userRole = _JWTSettings.Roles.FirstOrDefault(x => userRole.Contains(x.Key, StringComparison.CurrentCultureIgnoreCase)).Value;
            GroupDistinguishedName group = SearchGroup<GroupDistinguishedName>(userRole);
            searchFilter = $"(&{searchFilter}(memberOf={group.DistinguishedName.StringValue}))";
        }

        ILdapSearchResults searchResult = connection.Search(
            _LDAPSettings.BaseDN,
            LdapConnection.ScopeSub,
            searchFilter,
            GetQueryAttributes<TLdapUser>(),
            false,
            constraints
        );

        List<TLdapUser> ldapUsers = [];

        while (searchResult.HasMore())
        {
            LdapEntry entry = searchResult.Next();
            ldapUsers.Add(MapLdapEntry<TLdapUser>(entry));
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
    /// Escapes special characters in an LDAP search filter.
    /// </summary>
    /// <param name="value">The value to escape.</param>
    /// <returns>The escaped value.</returns>
    public static string EscapeLDAPSearchFilter(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        string[] specialChars = ["(", ")", "*", "\\", "\0"];

        foreach (string specialChar in specialChars)
        {
            value = value.Replace(specialChar, $"\\{specialChar}");
        }

        return value;
    }

    private static string EscapeGUID(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        return string.Concat(bytes.Select(b => $"\\{b:X2}"));
    }

    /// <summary>
    /// Retrieves the LDAP attributes to query based on the properties of type <typeparamref name="LdapMappedEntity"/> that have the <see cref="LDAPMapping"/> attribute.
    /// </summary>
    /// <typeparam name="LdapMappedEntity">The type to retrieve the LDAP attributes for.</typeparam>
    /// <returns>An array of LDAP attributes to query.</returns>
    private static string[] GetQueryAttributes<LdapMappedEntity>()
    {
        List<string> queryAttributes = [];

        foreach (PropertyInfo prop in typeof(LdapMappedEntity).GetProperties())
        {
            foreach (LDAPMapping attr in prop.GetCustomAttributes<LDAPMapping>())
            {
                queryAttributes.Add(attr.Name);
            }
        }

        return [.. queryAttributes];
    }

    /// <summary>
    /// Maps LDAP entries to objects of type <typeparamref name="LdapMappedEntity"/>.
    /// </summary>
    /// <typeparam name="LdapMappedEntity">The type to which the LDAP entry will be mapped. Must have a parameterless constructor.</typeparam>
    /// <param name="entry">The LDAP entry to be mapped.</param>
    /// <returns>An instance of <typeparamref name="LdapMappedEntity"/> with properties set based on the LDAP entry attributes.</returns>
    private static LdapMappedEntity MapLdapEntry<LdapMappedEntity>(LdapEntry entry) where LdapMappedEntity : new()
    {
        LdapMappedEntity mappedLdapResult = new();

        foreach (PropertyInfo prop in mappedLdapResult.GetType().GetProperties())
        {
            foreach (LDAPMapping attr in prop.GetCustomAttributes<LDAPMapping>())
            {
                prop.SetValue(mappedLdapResult, entry.GetAttribute(attr.Name));
            }
        }

        return mappedLdapResult;
    }

    private void WithSASL(string username, string password)
    {
        string FQDN = _LDAPSettings.FQDN;
        string realm = FQDN.Split('.', 2)[1];

        // realm = domain name of the AD server
        // host = The FQDN the server resides on, or one of its servicePrincipalName values
        connection.Bind(saslRequest: new SaslDigestMd5Request(username, password, realm, FQDN));
    }

    private void WithSimple(string username, string password)
    {
        string host = _LDAPSettings.FQDN.Split('.', 2)[1];
        username = $"{username}@{host}";
        
        // dn = The DistinguishedName of the object (user) to authenticate as.
        // UPN (User Principal Name) (username@domain),
        // and sAMAccountName (domain\username) also works.
        connection.Bind(username, password);
    }
}