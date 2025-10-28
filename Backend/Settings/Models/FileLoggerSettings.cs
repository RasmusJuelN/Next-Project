using System.ComponentModel;
using Microsoft.Extensions.Logging;
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
}
