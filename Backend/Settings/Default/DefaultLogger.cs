
namespace Settings.Default;

public class DefaultLogger : ILoggerSettings<DefaultConsoleLogger, DefaultFileLogger, DefaultDBLogger>
{
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Error },
        { "Microsoft", Microsoft.Extensions.Logging.LogLevel.Warning }
    };
    public DefaultConsoleLogger Console { get; set; } = new DefaultConsoleLogger();
    public DefaultFileLogger FileLogger { get; set; } = new DefaultFileLogger();
    public DefaultDBLogger DBLogger { get; set; } = new DefaultDBLogger();
}
