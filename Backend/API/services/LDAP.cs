using Settings.Models;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Sasl;
using API.Exceptions;

namespace API.Services;

public class LDAP(IConfiguration configuration)
{
    private readonly LDAPSettings LDAPSettings = new SettingsBinder(configuration).Bind<LDAPSettings>();
    public LdapConnection connection = new();

    public void Authenticate(string username, string password)
    {
        try
        {
            connection.Connect(LDAPSettings.Host, LDAPSettings.Port);

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
                    throw new LDAPException.ConnectionErrorException();
                }
                else if (e.ResultCode == 49)
                {
                    throw new LDAPException.InvalidCredentialsException();
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

    private void WithSASL(string username, string password)
    {
        string FQDN = LDAPSettings.FQDN;
        string realm = FQDN.Split('.', 2)[1];

        // realm = domain name of the AD server
        // host = The FQDN the server resides on, or one of its servicePrincipalName values
        connection.Bind(saslRequest: new SaslDigestMd5Request(username, password, realm, FQDN));
    }

    private void WithSimple(string username, string password)
    {
        string host = LDAPSettings.FQDN.Split('.', 2)[1];
        username = $"{username}@{host}";
        
        // dn = The DistinguishedName of the object (user) to authenticate as.
        // UPN (User Principal Name) (username@domain),
        // and sAMAccountName (domain\username) also works.
        connection.Bind(username, password);
    }
}
