
namespace Settings.Models;

public class DBLoggerSettings : Base, IDBLoggerSettings
{
    public override string Key { get; } = "DBLogging";

    public bool IsEnabled { get; set; }
    public Dictionary<string, LogLevel> LogLevel { get; set; } = [];
}
