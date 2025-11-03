using Microsoft.Extensions.Logging;
using Serilog;
using Settings.Interfaces;

namespace Settings.Default;

public class DefaultFileLogger : IFileLoggerSettings
{
    public bool IsEnabled { get; set; } = true;
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Warning },
        { "API", Microsoft.Extensions.Logging.LogLevel.Information },
        { "Database", Microsoft.Extensions.Logging.LogLevel.Information},
        { "Microsoft.EntityFrameworkCore.Migrations", Microsoft.Extensions.Logging.LogLevel.Information}
    };
    public string Path { get; set; } = "./app.log";
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;
    public bool RollOnFileSizeLimit { get; set; } = true;
    public int FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024;
    public int RetainedFileCountLimit { get; set; } = 30;
    public bool Shared { get; set; } = true;
}
