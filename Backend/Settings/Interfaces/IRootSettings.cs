using Settings.Models;

namespace Settings.Interfaces;

public interface IRootSettings
{
    public DatabaseSettings Database { get; set; }
    public JWTSettings JWT { get; set; }
    public LDAPSettings LDAP { get; set; }
    public LoggerSettings Logging { get; set; }
}
