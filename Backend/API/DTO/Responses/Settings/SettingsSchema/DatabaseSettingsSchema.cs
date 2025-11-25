using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.DTO.Responses.Settings.SettingsSchema;

public record class DatabaseSettingsSchema
{
    public required ConnectionStringSchema ConnectionString { get; set; }
}

public record class ConnectionStringSchema : SettingsSchemaBase
{
}