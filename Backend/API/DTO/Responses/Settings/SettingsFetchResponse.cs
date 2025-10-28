using Serilog;
using Settings.Interfaces;

namespace API.DTO.Responses.Settings;

public record class SettingsFetchResponse
{
    public required DatabaseSettingsFetchResponse Database { get; set; }
    public required JWTSettingsFetchResponse JWT { get; set; }
    public required LDAPSettingsFetchResponse LDAP { get; set; }
    public required LoggerSettingsFetchResponse Logging { get; set; }
    public required SystemSettingsFetchResponse System { get; set; }
}

public record class ConsoleLoggerSettingsFetchResponse : IConsoleLoggerSettings
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
}

public record class DatabaseSettingsFetchResponse : IDatabaseSettings
{
    public required string ConnectionString { get; set; }
}

public record class DBLoggerSettingsFetchResponse : IDBLoggerSettings
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
}

public record class FileLoggerSettingsFetchResponse : IFileLoggerSettings
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
    public required string Path { get; set; }
    public required RollingInterval RollingInterval { get; set; }
    public required bool RollOnFileSizeLimit { get; set; }
    public required int FileSizeLimitBytes { get; set; }
    public required int RetainedFileCountLimit { get; set; }
    public required bool Shared { get; set; }
}

public record class JWTSettingsFetchResponse : IJWTSettings
{
    public required string AccessTokenSecret { get; set; }
    public required string RefreshTokenSecret { get; set; }
    public required int TokenTTLMinutes { get; set; }
    public required int RenewTokenTTLDays { get; set; }
    public required Dictionary<string, string> Roles { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}

public record class LDAPSettingsFetchResponse : ILDAPSettings
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string FQDN { get; set; }
    public required string BaseDN { get; set; }
    public required string SA { get; set; }
    public required string SAPassword { get; set; }
}

public record class LoggerSettingsFetchResponse : ILoggerSettings<ConsoleLoggerSettingsFetchResponse, FileLoggerSettingsFetchResponse, DBLoggerSettingsFetchResponse>
{
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
    public required ConsoleLoggerSettingsFetchResponse Console { get; set; }
    public required FileLoggerSettingsFetchResponse FileLogger { get; set; }
    public required DBLoggerSettingsFetchResponse DBLogger { get; set; }
}

public record class SystemSettingsFetchResponse : ISystemSettings
{
    public required string ListenIP { get; set; }
    public required int HttpPort { get; set; }
    public required int HttpsPort { get; set; }
    public required bool UseSSL { get; set; }
    public required string PfxCertificatePath { get; set; }
}