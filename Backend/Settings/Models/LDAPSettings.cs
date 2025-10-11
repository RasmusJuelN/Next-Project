using Settings.Interfaces;

namespace Settings.Models;

public class LDAPSettings : Base, ILDAPSettings
{
    public override string Key { get; } = "LDAP";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public int SSLPort { get; set; } = 0;
    public bool UseSSL { get; set; } = false;
    public string FQDN { get; set; } = string.Empty;
    public string BaseDN { get; set; } = string.Empty;
    public string SA { get; set; } = string.Empty;
    public string SAPassword { get; set; } = string.Empty;
}
