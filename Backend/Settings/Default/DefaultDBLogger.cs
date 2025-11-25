
namespace Settings.Default;

public class DefaultDBLogger : IDBLoggerSettings
{
    public bool IsEnabled { get; set; } = true;
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Warning },
        { "API", Microsoft.Extensions.Logging.LogLevel.Information },
        { "Database", Microsoft.Extensions.Logging.LogLevel.Information},
        { "Microsoft.EntityFrameworkCore.Migrations", Microsoft.Extensions.Logging.LogLevel.Information}
    };
}
