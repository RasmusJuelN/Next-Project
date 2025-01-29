using Microsoft.Extensions.Logging;

namespace Settings.Interfaces;

public interface IConsoleLoggerSettings
{
    public bool IsEnabled { get; set; }
    public Dictionary<string, LogLevel> LogLevel { get; set; }
}
