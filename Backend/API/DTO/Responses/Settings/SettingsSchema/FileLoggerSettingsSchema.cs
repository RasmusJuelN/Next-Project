using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.DTO.Responses.Settings.SettingsSchema;

public record class FileLoggerSettingsSchema
{
    public required SettingsIsEnabledSchema IsEnabled { get; set; }
    public required SettingsLogLevelSchema LogLevel { get; set; }
    public required PathSchema Path { get; set; }
}

public record class PathSchema : SettingsSchemaBase
{ }