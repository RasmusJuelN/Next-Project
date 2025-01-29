using Microsoft.Extensions.Logging;
using Settings.Interfaces;

namespace Settings.Default;

public class DefaultLogger : ILoggerSettings<DefaultConsoleLogger, DefaultFileLogger>
{
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Error },
        { "Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning }
    };
    public DefaultConsoleLogger Console { get; set; } = new DefaultConsoleLogger();
    public DefaultFileLogger FileLogger { get; set; } = new DefaultFileLogger();
}
