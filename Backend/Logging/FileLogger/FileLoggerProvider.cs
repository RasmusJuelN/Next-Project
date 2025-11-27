
namespace Logging.FileLogger;

[UnsupportedOSPlatform("Browser")]
[ProviderAlias("FileLogger")]
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly IDisposable? _onChangeToken;
    private FileLoggerSettings _currentConfig;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = 
        new(StringComparer.OrdinalIgnoreCase);
    private readonly StreamWriter _writer;

    public FileLoggerProvider(IOptionsMonitor<FileLoggerSettings> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        _writer = new StreamWriter(new FileStream(_currentConfig.Path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) {AutoFlush = true};
    }
    
    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new FileLogger(name, _writer, GetCurrentConfig));

    private FileLoggerSettings GetCurrentConfig() => _currentConfig;

    public void Dispose()
    {
        _loggers.Clear();
        _onChangeToken?.Dispose();
    }
}
