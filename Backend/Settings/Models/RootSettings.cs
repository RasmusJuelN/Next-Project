using Settings.Interfaces;

namespace Settings.Models;

public class RootSettings : Base, IRootSettings
{
    public override string Key { get; } = "Settings";

    public DatabaseSettings Database { get; set; } = new();
    public JWTSettings JWT { get; set; } = new();
    public LDAPSettings LDAP { get; set; } = new();
    public LoggerSettings Logging { get; set; } = new();
    public SystemSettings System { get; set; } = new();
}
