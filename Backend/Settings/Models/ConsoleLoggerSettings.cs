using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class ConsoleLoggerSettings : Base, IConsoleLoggerSettings
{
    public override string Key { get; } = "Console";
    
    [Description("Indicates whether console logging is enabled.")]
    public bool IsEnabled { get; set; }

    [Description("Specifies the log levels for console logging.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];
}
