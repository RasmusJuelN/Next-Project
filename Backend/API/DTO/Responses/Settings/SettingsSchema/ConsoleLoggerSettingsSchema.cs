using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.DTO.Responses.Settings.SettingsSchema;

public record class ConsoleLoggerSettingsSchema
{
    public required SettingsIsEnabledSchema IsEnabled { get; set; }
    public required SettingsLogLevelSchema LogLevel { get; set; }
}
