
namespace Settings.Interfaces;

public interface IFileLoggerSettings
{
    public bool IsEnabled { get; set; }
    public Dictionary<string, LogLevel> LogLevel { get; set; }
    public string Path { get; set; }
}
