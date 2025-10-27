using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.DTO.Responses.Settings.SettingsSchema;

public record class LoggerSettingsSchema
{
    public required SettingsLogLevelSchema LogLevel { get; set; }
    public required ConsoleLoggerSettingsSchema ConsoleLogger { get; set; }
    public required FileLoggerSettingsSchema FileLogger { get; set; }
    public required DBLoggerSettingsSchema DBLogger { get; set; }
}
