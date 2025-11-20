using System.Text.Json.Serialization;
using Settings.Interfaces;

namespace Settings.Models;

public class RootSettings : Base, IRootSettings
{
    [JsonIgnore]
    public override string Key { get; } = "";

    public int Version { get; set; } = 2;

    public DatabaseSettings Database { get; set; } = new();
    public JWTSettings JWT { get; set; } = new();
    public LDAPSettings LDAP { get; set; } = new();
    public LoggerSettings Logging { get; set; } = new();
    public SystemSettings System { get; set; } = new();
}
