
namespace Settings.Interfaces;

public interface IDefaultSettings
{
    public DefaultDatabase Database { get; }
    public DefaultJWT JWT { get; }
    public DefaultLDAP LDAP { get; }
    public DefaultLogger Logging { get; }
    public DefaultSystem System { get; set; }
}
