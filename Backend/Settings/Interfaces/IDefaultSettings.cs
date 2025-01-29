using Settings.Default;

namespace Settings.Interfaces;

public interface IDefaultSettings
{
    public DefaultDatabase Database { get; }
    public DefaultJWT JWT { get; }
    public DefaultLDAP LDAP { get; }
    public DefaultLogger Logging { get; }
}
