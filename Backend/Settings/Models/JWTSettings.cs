
namespace Settings.Models;

public class JWTSettings : Base, IJWTSettings
{
    [JsonIgnore]
    public override string Key { get; } = "JWT";

    [Description("The secret key used to sign access tokens.")]
    public string AccessTokenSecret { get; set; } = RandomNumberGenerator.GetHexString(128);

    [Description("The secret key used to sign refresh tokens.")]
    public string RefreshTokenSecret { get; set; } = RandomNumberGenerator.GetHexString(128);

    [Description("The time-to-live (TTL) for access tokens in minutes.")]
    public int TokenTTLMinutes { get; set; } = 30;

    [Description("The time-to-live (TTL) for refresh tokens in days.")]
    public int RenewTokenTTLDays { get; set; } = 30;

    [Description("A dictionary mapping the internal roles to the roles or groups used by the user storage provider.")]
    public Dictionary<string, string> Roles { get; set; } = new Dictionary<string, string>() { { "student", "" }, {"teacher", ""}, {"admin", ""} };

    [Description("The issuer of the JWT tokens.")]
    public string Issuer { get; set; } = string.Empty;

    [Description("The intended audience for the JWT tokens.")]
    public string Audience { get; set; } = string.Empty;
}
