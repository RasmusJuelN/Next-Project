
namespace Logging.Extensions;

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFileLogger(
        this ILoggingBuilder builder)
    {
        builder.AddConfiguration();

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>());

        LoggerProviderOptions.RegisterProviderOptions
            <DefaultFileLogger, FileLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddFileLogger(
        this ILoggingBuilder builder,
        Action<DefaultFileLogger> configure)
        {
            builder.AddFileLogger();
            builder.Services.Configure(configure);

            return builder;
        }
}
