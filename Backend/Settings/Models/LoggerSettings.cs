using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class LoggerSettings : Base, ILoggerSettings<ConsoleLoggerSettings, FileLoggerSettings, DBLoggerSettings>
{
    [JsonIgnore]
    public override string Key { get; } = "Logging";
    
    [Description("Defines the default global log levels for the application.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Error },
        { "Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning }
    };
    public ConsoleLoggerSettings Console { get; set; } = new ConsoleLoggerSettings();
    public FileLoggerSettings FileLogger { get; set; } = new FileLoggerSettings();
    public DBLoggerSettings DBLogger { get; set; } = new DBLoggerSettings();
}
