using Settings.Models;
using Novell.Directory.Ldap;
using Novell.Directory.Ldap.Sasl;

namespace API.services;

internal class LDAP
{
    private readonly LDAPSettings lDAPSettings;
    internal LDAP(IConfiguration configuration)
    {
        lDAPSettings = new SettingsBinder(configuration).Bind<LDAPSettings>();
    }

    internal bool Authenticate(string username, string password)
    {
        using LdapConnection cn = new();
        cn.Connect($"ldap://{lDAPSettings.Host}", lDAPSettings.Port);
        cn.Bind(saslRequest: new SaslDigestMd5Request(username, password, null, lDAPSettings.Host));
        return cn.Bound;
    }
}
