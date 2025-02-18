using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Settings.Default;

namespace Logging.FileLogger;

[UnsupportedOSPlatform("Browser")]
[ProviderAlias("FileLogger")]
public sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly IDisposable? _onChangeToken;
    private DefaultFileLogger _currentConfig;
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = 
        new(StringComparer.OrdinalIgnoreCase);
    private readonly StreamWriter _writer;

    public FileLoggerProvider(IOptionsMonitor<DefaultFileLogger> config)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        _writer = new StreamWriter(new FileStream(_currentConfig.Path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)) {AutoFlush = true};
    }
    
    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new FileLogger(name, _writer, GetCurrentConfig));

    private DefaultFileLogger GetCurrentConfig() => _currentConfig;

    public void Dispose()
    {
        _loggers.Clear();
        _onChangeToken?.Dispose();
    }
}
