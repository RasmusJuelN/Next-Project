namespace API.DTO.Requests.Settings;

public class PatchSettingsRequest
{
    public DatabasePatchRequest? Database { get; set; }
    public JWTPatchRequest? JWT { get; set; }
    public LDAPPatchRequest? LDAP { get; set; }
    public LoggerPatchRequest? Logging { get; set; }
    public SystemPatchRequest? System { get; set; }
}

public class DatabasePatchRequest
{
    public string? ConnectionString { get; set; }
}

public class JWTPatchRequest
{
    public string? AccessTokenSecret { get; set; }
    public string? RefreshTokenSecret { get; set; }
    public int? TokenTTLMinutes { get; set; }
    public int? RenewTokenTTLDays { get; set; }
    public Dictionary<string, string>? Roles { get; set; }
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
}

public class LDAPPatchRequest
{
    public string? Host { get; set; }
    public int? Port { get; set; }
    public string? FQDN { get; set; }
    public string? BaseDN { get; set; }
    public string? SA { get; set; }
    public string? SAPassword { get; set; }
}

public class LoggerPatchRequest
{
    public Dictionary<string, LogLevel>? LogLevel { get; set; }
    public ConsoleLoggerPatchRequest? Console { get; set; }
    public FileLoggerPatchRequest? FileLogger { get; set; }
    public DBLoggerPatchRequest? DBLogger { get; set; }
}

public class ConsoleLoggerPatchRequest
{
    public bool? IsEnabled { get; set; }
    public Dictionary<string, LogLevel>? LogLevel { get; set; }
}

public class FileLoggerPatchRequest
{
    public bool? IsEnabled { get; set; }
    public Dictionary<string, LogLevel>? LogLevel { get; set; }
    public string? Path { get; set; }
}

public class DBLoggerPatchRequest
{
    public bool? IsEnabled { get; set; }
    public Dictionary<string, LogLevel>? LogLevel { get; set; }
}

public class SystemPatchRequest
{
    public string? ListenIP { get; set; }
    public int? HttpPort { get; set; }
    public int? HttpsPort { get; set; }
    public bool? UseSSL { get; set; }
    public string? PfxCertificatePath { get; set; }
}