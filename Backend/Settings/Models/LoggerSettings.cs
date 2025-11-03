using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class LoggerSettings : Base, ILoggerSettings<ConsoleLoggerSettings, FileLoggerSettings, DBLoggerSettings>
{
    public override string Key { get; } = "Logging";
    
    [Description("Defines the default global log levels for the application.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];
    public ConsoleLoggerSettings Console { get; set; } = new ConsoleLoggerSettings();
    public FileLoggerSettings FileLogger { get; set; } = new FileLoggerSettings();
    public DBLoggerSettings DBLogger { get; set; } = new DBLoggerSettings();
}
