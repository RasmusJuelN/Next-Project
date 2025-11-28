
namespace Logging.DBLogger;

public sealed class DBLogger(
    string categoryName,
    BlockingCollection<ApplicationLog> logQueue,
    Func<DBLoggerSettings> getCurrentConfig) : ILogger
{
    private readonly string _categoryName = categoryName;
    private readonly BlockingCollection<ApplicationLog> _logQueue = logQueue;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel)
    {
        DBLoggerSettings config = getCurrentConfig();
        if (!config.IsEnabled) return false;
        
        // Prevent circular logging by excluding most Entity Framework logs
        // Allow only migrations to be logged to avoid losing important schema changes
        if (_categoryName.StartsWith("Microsoft.EntityFrameworkCore") && 
            !_categoryName.StartsWith("Microsoft.EntityFrameworkCore.Migrations"))
        {
            return false;
        }
        
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

        ApplicationLog log = new()
        {
            Message = message,
            LogLevel = logLevel,
            EventId = eventId.Id,
            EventDescription = eventId.Name ?? string.Empty,
            Category = _categoryName,
            Exception = exception?.ToString() ?? string.Empty
        };

        _logQueue.Add(log);
    }
}
