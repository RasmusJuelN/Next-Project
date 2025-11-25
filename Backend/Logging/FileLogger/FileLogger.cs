
namespace Logging.FileLogger;

public sealed class FileLogger(
    string categoryName,
    StreamWriter writer,
    Func<DefaultFileLogger> getCurrentConfig) : ILogger
{
    private readonly string _categoryName = categoryName;
    private readonly StreamWriter _writer = writer;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        DefaultFileLogger config = getCurrentConfig();
        if (!config.IsEnabled) return false;
        
        // Check if the category name is in the LogLevel dictionary
        // i.e. if the category name is "API", check if "API" is in the dictionary
        if (config.LogLevel.TryGetValue(_categoryName, out LogLevel minLogLevel))
        {
            return logLevel >= minLogLevel;
        }

        // Check if the category name has a parent category in the LogLevel dictionary
        // i.e. if the category name is "Microsoft.AspNetCore", check if "Microsoft" is in the dictionary
        string categoryName = _categoryName;
        while (categoryName.Contains('.'))
        {
            categoryName = categoryName[..categoryName.LastIndexOf('.')];
            if (config.LogLevel.TryGetValue(categoryName, out minLogLevel))
            {
                return logLevel >= minLogLevel;
            }
        }

        // If the category name is not in the LogLevel dictionary, use the default LogLevel
        return logLevel >= config.LogLevel["Default"];
    }
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        string message = formatter(state, exception);
        if (string.IsNullOrEmpty(message)) return;

        string logEntry = $"[{DateTime.UtcNow}] [{logLevel}] {_categoryName}:  {message}";
        if (exception != null)
        {
            logEntry += Environment.NewLine + $"Exception: {exception}";
        }

        _writer.WriteLine(logEntry);
    }
}
