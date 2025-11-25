
namespace Settings.Default;

public class DefaultConsoleLogger : IConsoleLoggerSettings
{
    public bool IsEnabled { get; set; } = true;
    public Dictionary<string, LogLevel> LogLevel { get; set; } = new Dictionary<string, LogLevel>
    {
        { "Default", Microsoft.Extensions.Logging.LogLevel.Warning },
        { "Microsoft.Hosting.Lifetime", Microsoft.Extensions.Logging.LogLevel.Information }
    };
}
