using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class LoggerSettings : Base, ILoggerSettings<ConsoleLoggerSettings, FileLoggerSettings>
{
    public override string Key { get; } = "Logging";
    
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];
    public ConsoleLoggerSettings Console { get; set; } = new ConsoleLoggerSettings();
    public FileLoggerSettings FileLogger { get; set; } = new FileLoggerSettings();
}
