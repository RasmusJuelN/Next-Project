namespace API.DTO.Requests.Settings;

public class UpdateSettingsRequest
{
    public required DatabaseUpdateRequest Database { get; set; }
    public required JWTUpdateRequest JWT { get; set; }
    public required LDAPUpdateRequest LDAP { get; set; }
    public required LoggerUpdateRequest Logging { get; set; }
    public required SystemUpdateRequest System { get; set; }
}

public class DatabaseUpdateRequest
{
    public required string ConnectionString { get; set; }
}

public class JWTUpdateRequest
{
    public required string AccessTokenSecret { get; set; }
    public required string RefreshTokenSecret { get; set; }
    public required int TokenTTLMinutes { get; set; }
    public required int RenewTokenTTLDays { get; set; }
    public required Dictionary<string, string> Roles { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
}

public class LDAPUpdateRequest
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string FQDN { get; set; }
    public required string BaseDN { get; set; }
    public required string SA { get; set; }
    public required string SAPassword { get; set; }
}

public class LoggerUpdateRequest
{
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
    public required ConsoleLoggerUpdateRequest Console { get; set; }
    public required FileLoggerUpdateRequest FileLogger { get; set; }
    public required DBLoggerUpdateRequest DBLogger { get; set; }
}

public class ConsoleLoggerUpdateRequest
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
}

public class FileLoggerUpdateRequest
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
    public required string Path { get; set; }
}

public class DBLoggerUpdateRequest
{
    public required bool IsEnabled { get; set; }
    public required Dictionary<string, LogLevel> LogLevel { get; set; }
}

public class SystemUpdateRequest
{
    public required string ListenIP { get; set; }
    public required int HttpPort { get; set; }
    public required int HttpsPort { get; set; }
    public required bool UseSSL { get; set; }
    public required string PfxCertificatePath { get; set; }
}