namespace Settings.Interfaces;

public interface IJWTSettings
{
    public string Secret { get; set; }
    public string Algorithm { get; set; }
    public int TokenTTLMinutes { get; set; }
    public Dictionary<string, string> Roles { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
