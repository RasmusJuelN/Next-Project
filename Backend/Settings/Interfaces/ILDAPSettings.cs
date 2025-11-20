namespace Settings.Interfaces;

public interface ILDAPSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public int SSLPort { get; set; }
    public bool UseSSL { get; set; }
    public string FQDN { get; set; }
    public string BaseDN { get; set; }
    public string SA { get; set; }
    public string SAPassword { get; set; }
}
