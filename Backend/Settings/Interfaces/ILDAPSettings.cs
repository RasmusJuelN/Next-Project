namespace Settings.Interfaces;

public interface ILDAPSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string BaseDN { get; set; }
    public string SA { get; set; }
    public string SAPassword { get; set; }
}
