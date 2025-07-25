using Settings.Interfaces;

namespace Settings.Models;

public class JWTSettings : Base, IJWTSettings
{
    public override string Key { get; } = "JWT";

    public string AccessTokenSecret { get; set; } = string.Empty;
    public string RefreshTokenSecret { get; set; } = string.Empty;
    public int TokenTTLMinutes { get; set; } = 0;
    public int RenewTokenTTLDays { get; set; } = 0;
    public Dictionary<string, string> Roles { get; set; } = [];
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
