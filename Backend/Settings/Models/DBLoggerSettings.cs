
namespace Settings.Models;

public class DBLoggerSettings : Base, IDBLoggerSettings
{
    [JsonIgnore]
    public override string Key { get; } = "DBLogging";

    [Description("Indicates whether database logging is enabled.")]
    public bool IsEnabled { get; set; } = true;

    [Description("Specifies the log levels for database logging.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Warning },
        { "API", Microsoft.Extensions.Logging.LogLevel.Information },
        { "Database", Microsoft.Extensions.Logging.LogLevel.Information},
        { "Microsoft.EntityFrameworkCore.Migrations", Microsoft.Extensions.Logging.LogLevel.Information}
    };
}
