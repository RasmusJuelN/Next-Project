using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Serilog;
using Settings.Interfaces;

namespace Settings.Models;

public class FileLoggerSettings : Base, IFileLoggerSettings
{
    [JsonIgnore]
    public override string Key { get; } = "FileLogging";
    
    [Description("Indicates whether file logging is enabled.")]
    public bool IsEnabled { get; set; } = true;

    [Description("Specifies the log levels for file logging.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Warning },
        { "API", Microsoft.Extensions.Logging.LogLevel.Information },
        { "Database", Microsoft.Extensions.Logging.LogLevel.Information},
        { "Microsoft.EntityFrameworkCore.Migrations", Microsoft.Extensions.Logging.LogLevel.Information}
    };

    [Description("Specifies the file path where log files will be stored.")]
    public string Path { get; set; } = "./logs/app-.log";

    [Description("Specifies the rolling interval for log files.")]
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Day;

    [Description("Indicates whether to roll the log file on size limit.")]
    public bool RollOnFileSizeLimit { get; set; } = true;

    [Description("Specifies the maximum size in bytes for a log file before it is rolled over.")]
    public int FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024;

    [Description("Specifies the maximum number of retained log files.")]
    public int RetainedFileCountLimit { get; set; } = 30;

    [Description("Indicates whether the log file is shared between multiple processes.")]
    public bool Shared { get; set; } = true;
}
