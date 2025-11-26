
namespace Settings.Interfaces;

public interface IFileLoggerSettings
{
    public bool IsEnabled { get; set; }
    public Dictionary<string, LogLevel> LogLevel { get; set; }
    public string Path { get; set; }
    public RollingInterval RollingInterval { get; set; }
    public bool RollOnFileSizeLimit { get; set; }
    public int FileSizeLimitBytes { get; set; }
    public int RetainedFileCountLimit { get; set; }
    public bool Shared { get; set; }
}
