using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Database.Models;
using Database.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Settings.Default;

namespace Logging.DBLogger;

[UnsupportedOSPlatform("Browser")]
[ProviderAlias("DBLogger")]
public sealed class DBLoggerProvider : ILoggerProvider
{
    private readonly IDisposable? _onChangeToken;
    private DefaultDBLogger _currentConfig;
    private readonly ConcurrentDictionary<string, DBLogger> _loggers =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly BlockingCollection<ApplicationLogsModel> _logQueue = new( new ConcurrentQueue<ApplicationLogsModel>(), 1000);
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _outputTask;
    private readonly IServiceProvider _serviceProvider;

    public DBLoggerProvider(IOptionsMonitor<DefaultDBLogger> config, IServiceProvider serviceProvider)
    {
        _currentConfig = config.CurrentValue;
        _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        _serviceProvider = serviceProvider;
        _outputTask = Task.Run(ProcessLogQueue);
    }

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new DBLogger(name, _logQueue, GetCurrentConfig));

    private DefaultDBLogger GetCurrentConfig() => _currentConfig;

    public void Dispose()
    {
        _logQueue.CompleteAdding();
        _cts.Cancel();
        _outputTask.Wait();
        _logQueue.Dispose();
        _cts.Dispose();
        _onChangeToken?.Dispose();
    }

    private async Task ProcessLogQueue()
    {
        try
        {
            // Keep processing log entries until the token is cancelled
            while (!_cts.Token.IsCancellationRequested)
            {
                List<ApplicationLogsModel> logs = [];

                while (_logQueue.TryTake(out ApplicationLogsModel? log, TimeSpan.FromSeconds(1)))
                {
                    if (log != null)
                    {
                        logs.Add(log);
                    }
                }

                if (logs.Count > 0)
                {
                    // ILoggerProvider is a singleton, so we need to create a new scope to resolve scoped services
                    using IServiceScope scope = _serviceProvider.CreateScope();
                    IGenericRepository<ApplicationLogsModel> repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<ApplicationLogsModel>>();
                    await SaveLogsToDatabase(logs, repository);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore, the while loop will exit
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private static async Task SaveLogsToDatabase(List<ApplicationLogsModel> logs, IGenericRepository<ApplicationLogsModel> repository)
    {
        await repository.AddRangeAsync(logs);
    }
}
