using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class ConsoleLoggerSettings : Base, IConsoleLoggerSettings
{
    public override string Key { get; } = "Console";
    
    public bool IsEnabled { get; set; }
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];
}
