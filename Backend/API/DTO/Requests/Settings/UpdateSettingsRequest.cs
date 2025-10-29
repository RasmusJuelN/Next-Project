using Serilog;
using Settings.Interfaces;

namespace API.DTO.Requests.Settings;

public class UpdateSettingsRequest
{
    public required DatabaseUpdateRequest Database { get; set; }
    public required JWTUpdateRequest JWT { get; set; }
    public required LDAPUpdateRequest LDAP { get; set; }
    public required LoggerUpdateRequest Logging { get; set; }
    public required SystemUpdateRequest System { get; set; }
}

public class DatabaseUpdateRequest : IDatabaseSettings
{
    public required string ConnectionString { get; set; }
}

public class JWTUpdateRequest : IJWTSettings
{
    public required string AccessTokenSecret { get; set; }
    public required string RefreshTokenSecret { get; set; }
    public required int TokenTTLMinutes { get; set; }
    public required int RenewTokenTTLDays { get; set; }
    public required Dictionary<string, string> Roles { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}

public class LDAPUpdateRequest : ILDAPSettings
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string FQDN { get; set; }
    public required string BaseDN { get; set; }
    public required string SA { get; set; }
    public required string SAPassword { get; set; }
}

public class LoggerUpdateRequest : ILoggerSettings<ConsoleLoggerUpdateRequest, FileLoggerUpdateRequest, DBLoggerUpdateRequest>
{
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
    public required ConsoleLoggerUpdateRequest Console { get; set; }
    public required FileLoggerUpdateRequest FileLogger { get; set; }
    public required DBLoggerUpdateRequest DBLogger { get; set; }
}

public class ConsoleLoggerUpdateRequest : IConsoleLoggerSettings
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
}

public class FileLoggerUpdateRequest : IFileLoggerSettings
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
    public required string Path { get; set; }
    public RollingInterval RollingInterval { get; set; }
    public bool RollOnFileSizeLimit { get; set; }
    public int FileSizeLimitBytes { get; set; }
    public int RetainedFileCountLimit { get; set; }
    public bool Shared { get; set; }
}

public class DBLoggerUpdateRequest : IDBLoggerSettings
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
}

public class SystemUpdateRequest : ISystemSettings
{
    public required string ListenIP { get; set; }
    public required int HttpPort { get; set; }
    public required int HttpsPort { get; set; }
    public required bool UseSSL { get; set; }
    public required string PfxCertificatePath { get; set; }
}