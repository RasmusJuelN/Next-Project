using Logging.DBLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Settings.Default;

namespace Logging.Extensions;

public static class DBLoggerExtensions
{
    public static ILoggingBuilder AddDBLogger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, DBLoggerProvider>());
        
        LoggerProviderOptions.RegisterProviderOptions
            <DefaultDBLogger, DBLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddDBLogger(
        this ILoggingBuilder builder,
        Action<DefaultDBLogger> configure)
        {
            builder.AddDBLogger();
            builder.Services.Configure(configure);

            return builder;
        }
}
