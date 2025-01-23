using Settings.Interfaces;

namespace Settings.Default;

public class DefaultJWT : IJWTSettings
{
    public string Secret { get; set; } = string.Empty;
    public int TokenTTLMinutes { get; set; } = 30;
    public int RenewTokenTTLDays { get; set; } = 30;
    public Dictionary<string, string> Roles { get; set; } = [];
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
