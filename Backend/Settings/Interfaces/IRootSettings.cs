
namespace Settings.Interfaces;

public interface IRootSettings
{
    public int Version { get; set; }

    public DatabaseSettings Database { get; set; }
    public JWTSettings JWT { get; set; }
    public LDAPSettings LDAP { get; set; }
    public LoggerSettings Logging { get; set; }
    public SystemSettings System { get; set; }
}
