using Settings.Models;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Sasl;
using API.Exceptions;

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

    public Guid GetObjectGuid()
    {
        if (!connection.Bound) throw new LDAPException.NotBound(
            "A bound connection is required. Please first use the authorization method."
            );

        LdapWhoAmIResponse whoAmI = connection.WhoAmI();

        string domain = _LDAPSettings.FQDN.Split(".", 2).Last();
        
        // DOMAIN\\username
        string sAMAccountName = EscapeLDAPSearchFilter(whoAmI.AuthzIdWithoutType);
        // username@domain
        string userPrincipalName = $"{sAMAccountName.Split('\\').Last()}@{domain}";
        
        string searchFilter = $"(|(userPrincipalName={userPrincipalName})(sAMAccountName={sAMAccountName}))";

        ILdapSearchResults searchResults = connection.Search(
            "OU=users,OU=next," + _LDAPSettings.BaseDN,
            LdapConnection.ScopeSub,
            searchFilter,
            ["objectGUID"],
            false
        );

        LdapEntry entry = searchResults.Next();
        LdapAttribute objectGUID = entry.GetAttribute("objectGUID");

        return new Guid(objectGUID.ByteValue);
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

    private string EscapeLDAPSearchFilter(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        string[] specialChars = ["(", ")", "*", "\\", "\0"];

        foreach (string specialChar in specialChars)
        {
            value = value.Replace(specialChar, $"\\{specialChar}");
        }

        return value;
    }
}
