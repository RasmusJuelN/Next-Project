using Settings.Models;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Sasl;
using API.Exceptions;
using System.Reflection;
using API.Attributes;

namespace API.Services;

public class LDAP
{
    private readonly LDAPSettings _LDAPSettings;
    public LdapConnection connection = new();

    public LDAP(IConfiguration configuration)
    {
        _LDAPSettings = new SettingsBinder(configuration).Bind<LDAPSettings>();
        LdapSearchConstraints constraints = connection.SearchConstraints;
        constraints.ReferralFollowing = true;
        connection.Constraints = constraints;
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
    /// Searches the LDAP directory using the specified search query and maps the results to a list of objects of type <typeparamref name="T"/>.
    /// <para></para>
    /// IMPORTANT: Please ensure any special characters that may need to be used in the query itself are properly escaped.
    /// They can either be escaped manually with <c>\\</c>, or by running the value you're searching for through <see cref="EscapeLDAPSearchFilter"/>
    /// <para></para>
    /// LDAP supports AND (<c>&amp;</c>), OR (<c>|</c>), Wildcards (<c>*</c>), Equality (<c>=</c>), 
    /// Negation (<c>!</c>), and Greater/Less than (<c>&gt;</c>|<c>&lt;</c>).
    /// <para></para>
    /// Examples:
    /// <para></para>
    /// <c>(|(userPrincipalName=Nick)(sAMAccountName=Nick))</c> = userPrincipalName OR sAMAccountName is Nick.
    /// <para></para>
    /// <c>(!(userPrincipalName=Nick))</c> = userPrincipalName is NOT Nick.
    /// <para></para>
    /// <c>(userPrincipalName=*)</c> = Any object which has a value in userPrincipalName. Please don't actually do that :(
    /// <para></para>
    /// <c>(userPrincipalName=*Nick*)</c> = Any object where "Nick" is in the value in userPrincipalName.
    /// <para></para>
    /// https://www.ldapexplorer.com/en/manual/109010000-ldap-filter-syntax.htm can be refered to for more insight.
    /// </summary>
    /// <typeparam name="T">The type of objects to map the LDAP search results to. Must have a parameterless constructor.</typeparam>
    /// <param name="searchQuery">The LDAP search query.</param>
    /// <param name="baseDN">The base distinguished name (DN) to search from.</param>
    /// <param name="scope">The scope of the search. Defaults to <see cref="LdapConnection.ScopeSub"/>.</param>
    /// <param name="attributes">The attributes to retrieve. Defaults to all attributes if not specified.</param>
    /// <returns>A list of objects of type <typeparamref name="T"/> mapped from the LDAP search results.</returns>
    /// <exception cref="LDAPException.NotBound">Thrown if the LDAP connection is not bound.</exception>
    public List<T> SearchLDAP<T>(string searchQuery, string baseDN, int scope = LdapConnection.ScopeSub) where T : new()
    {
        if (!connection.Bound) throw new LDAPException.NotBound(
            "A bound connection is required. Please first use the authorization method."
            );
        
        string[] attributes = GetQueryAttributes<T>();

        ILdapSearchResults searchResults = connection.Search(
            baseDN,
            scope,
            searchQuery,
            attributes,
            false
        );

        List<T> mappedLdapResults = [];

        while (searchResults.HasMore())
        {
            LdapEntry entry = searchResults.Next();

            mappedLdapResults.Add(MapLdapEntry<T>(entry));
        }
        
        return mappedLdapResults;
    }

    /// <summary>
    /// Searches the LDAP directory for a user with the specified username and maps the results to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to map the LDAP search results to. Must have a parameterless constructor.</typeparam>
    /// <returns>An object of type <typeparamref name="T"/> mapped from the LDAP search results.</returns>
    /// <exception cref="LDAPException.NotBound">Thrown if the LDAP connection is not bound.</exception>
    public T SearchUser<T>(string username) where T : new()
    {
        string domain = _LDAPSettings.FQDN.Split(".", 2).Last();
        
        // NETBIOS\\username
        string sAMAccountName = $"{string.Join("", domain.Split('.'))}\\\\{username}";
        // username@domain
        string userPrincipalName = $"{sAMAccountName.Split('\\').Last()}@{domain}";
        
        string searchFilter = $"(|(userPrincipalName={userPrincipalName})(sAMAccountName={sAMAccountName}))";

        T userSearch = SearchLDAP<T>(searchFilter, _LDAPSettings.BaseDN, LdapConnection.ScopeSub).First();

        return userSearch;
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

    /// <summary>
    /// Retrieves the LDAP attributes to query based on the properties of type <typeparamref name="T"/> that have the <see cref="LDAPMapping"/> attribute.
    /// </summary>
    /// <typeparam name="T">The type to retrieve the LDAP attributes for.</typeparam>
    /// <returns>An array of LDAP attributes to query.</returns>
    private static string[] GetQueryAttributes<T>()
    {
        List<string> queryAttributes = [];

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            foreach (LDAPMapping attr in prop.GetCustomAttributes<LDAPMapping>())
            {
                queryAttributes.Add(attr.Name);
            }
        }

        return [.. queryAttributes];
    }

    /// <summary>
    /// Maps LDAP entries to objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to which the LDAP entry will be mapped. Must have a parameterless constructor.</typeparam>
    /// <param name="entry">The LDAP entry to be mapped.</param>
    /// <returns>An instance of <typeparamref name="T"/> with properties set based on the LDAP entry attributes.</returns>
    private static T MapLdapEntry<T>(LdapEntry entry) where T : new()
    {
        T mappedLdapResult = new();

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
