using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.DTO.Responses.Settings.SettingsSchema;

public record class FileLoggerSettingsSchema
{
    public required SettingsIsEnabledSchema IsEnabled { get; set; }
    public required SettingsLogLevelSchema LogLevel { get; set; }
    public required PathSchema Path { get; set; }
    public required RollingIntervalSchema RollingInterval { get; set; }
    public required RollOnFileSizeLimitSchema RollOnFileSizeLimit { get; set; }
    public required FileSizeLimitBytesSchema FileSizeLimitBytes { get; set; }
    public required RetainedFileCountLimitSchema RetainedFileCountLimit { get; set; }
    public required SharedSchema Shared { get; set; }
}

public record class PathSchema : SettingsSchemaBase
{ }

public record class RollingIntervalSchema : SettingsSchemaExtended
{ }

public record class RollOnFileSizeLimitSchema : SettingsSchemaExtended
{ }

public record class FileSizeLimitBytesSchema : SettingsSchemaExtended
{ }

public record class RetainedFileCountLimitSchema : SettingsSchemaExtended
{ }

public record class SharedSchema : SettingsSchemaExtended
{ }