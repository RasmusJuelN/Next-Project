using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class FileLoggerSettings : Base, IFileLoggerSettings
{
    public override string Key { get; } = "FileLogging";
    
    public bool IsEnabled { get; set; }
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];
    public string Path { get; set; } = string.Empty;
}
