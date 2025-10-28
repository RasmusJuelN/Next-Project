using System.ComponentModel;
using Settings.Interfaces;

namespace Settings.Models;

public class LDAPSettings : Base, ILDAPSettings
{
    public override string Key { get; } = "LDAP";

    [Description("The LDAP server host address. Can be an IP address or domain name.")]
    public string Host { get; set; } = string.Empty;

    [Description("The LDAP server port number.")]
    public int Port { get; set; } = 0;

    [Description("The Fully Qualified Domain Name for the LDAP server.")]
    public string FQDN { get; set; } = string.Empty;

    [Description("The Base Distinguished Name (DN) for LDAP searches.")]
    public string BaseDN { get; set; } = string.Empty;

    [Description("The Service Account (SA) username for LDAP authentication.")]
    public string SA { get; set; } = string.Empty;

    [Description("The Service Account (SA) password for LDAP authentication.")]
    public string SAPassword { get; set; } = string.Empty;
}
