namespace API.DTO.Responses.Settings.SettingsSchema;

public record class SettingsSchema
{
    public required DatabaseSettingsSchema Database { get; set; }
    public required JWTSettingsSchema JWT { get; set; }
    public required LDAPSettingsSchema LDAP { get; set; }
    public required LoggerSettingsSchema Logging { get; set; }
    public required SystemSettingsSchema System { get; set; }
}
