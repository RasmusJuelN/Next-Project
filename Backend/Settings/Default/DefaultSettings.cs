
namespace Settings.Default;

public class DefaultSettings : IDefaultSettings
{
    public DefaultDatabase Database { get; } = new();
    public DefaultJWT JWT { get; } = new();
    public DefaultLDAP LDAP { get; } = new();
    public DefaultLogger Logging { get; } = new();
    public DefaultSystem System { get; set; } = new();
}
