using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Serilog;
using Settings.Interfaces;

namespace Settings.Models;

public class FileLoggerSettings : Base, IFileLoggerSettings
{
    public override string Key { get; } = "FileLogging";
    
    [Description("Indicates whether file logging is enabled.")]
    public bool IsEnabled { get; set; }

    [Description("Specifies the log levels for file logging.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];

    [Description("Specifies the file path where log files will be stored.")]
    public string Path { get; set; } = string.Empty;

    [Description("Specifies the rolling interval for log files.")]
    public RollingInterval RollingInterval { get; set; }

    [Description("Indicates whether to roll the log file on size limit.")]
    public bool RollOnFileSizeLimit { get; set; }
    
    [Description("Specifies the maximum size in bytes for a log file before it is rolled over.")]
    public int FileSizeLimitBytes { get; set; }

    [Description("Specifies the maximum number of retained log files.")]
    public int RetainedFileCountLimit { get; set; }

    [Description("Indicates whether the log file is shared between multiple processes.")]
    public bool Shared { get; set; }
}
