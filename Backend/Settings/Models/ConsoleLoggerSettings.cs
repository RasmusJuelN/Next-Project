using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Models;

public class ConsoleLoggerSettings : Base, IConsoleLoggerSettings
{
    [JsonIgnore]
    public override string Key { get; } = "Console";
    
    [Description("Indicates whether console logging is enabled.")]
    public bool IsEnabled { get; set; } = true;

    [Description("Specifies the log levels for console logging.")]
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Warning },
        { "Microsoft.Hosting.Lifetime", Microsoft.Extensions.Logging.LogLevel.Information }
    };
}
