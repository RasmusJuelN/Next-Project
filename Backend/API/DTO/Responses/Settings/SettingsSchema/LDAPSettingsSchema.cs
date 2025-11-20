using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.DTO.Responses.Settings.SettingsSchema;

public record class LDAPSettingsSchema
{
    public required HostSchema Host { get; set; }
    public required PortSchema Port { get; set; }
    public required FQDNSchema FQDN { get; set; }
    public required BaseDNSchema BaseDN { get; set; }
    public required SAUsernameSchema SAUsername { get; set; }
    public required SAPasswordSchema SAPassword { get; set; }
}

public record class HostSchema : SettingsSchemaBase
{ }

public record class PortSchema : SettingsSchemaExtended
{ }

public record class FQDNSchema : SettingsSchemaBase
{ }

public record class BaseDNSchema : SettingsSchemaBase
{ }

public record class SAUsernameSchema : SettingsSchemaBase
{ }

public record class SAPasswordSchema : SettingsSchemaBase
{ }