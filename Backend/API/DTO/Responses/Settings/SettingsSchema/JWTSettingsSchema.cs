using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.DTO.Responses.Settings.SettingsSchema;

public record class JWTSettingsSchema
{
    public required AccessTokenSecretSchema AccessTokenSecret { get; set; }
    public required RefreshTokenSecretSchema RefreshTokenSecret { get; set; }
    public required TokenTTLMinutesSchema TokenTTLMinutes { get; set; }
    public required RenewTokenTTLDaysSchema RenewTokenTTLDays { get; set; }
    public required RolesSchema Roles { get; set; }
    public required IssuerSchema Issuer { get; set; }
    public required AudienceSchema Audience { get; set; }
}

public record class AccessTokenSecretSchema : SettingsSchemaBase
{ }

public record class RefreshTokenSecretSchema : SettingsSchemaBase
{ }

public record class TokenTTLMinutesSchema : SettingsSchemaExtended
{ }

public record class RenewTokenTTLDaysSchema : SettingsSchemaExtended
{ }

public record class RolesSchema : SettingsSchemaExtended
{ }

public record class IssuerSchema : SettingsSchemaBase
{ }

public record class AudienceSchema : SettingsSchemaBase
{ }