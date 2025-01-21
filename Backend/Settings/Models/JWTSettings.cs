using Settings.Interfaces;

namespace Settings.Models;

public class JWTSettings : Base, IJWTSettings
{
    public override string Key { get; } = "JWT";

    public string Secret { get; set; } = string.Empty;
    public string Algorithm { get; set; } = string.Empty;
    public int TokenTTLMinutes { get; set; } = 0;
    public Dictionary<string, string> Roles { get; set; } = [];
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
