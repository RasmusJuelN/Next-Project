using System.Reflection;
using System.Text.Json;
using API.DTO.Responses.Settings;
using API.DTO.Responses.Settings.SettingsSchema;
using API.DTO.Responses.Settings.SettingsSchema.Bases;
using Settings.Default;
using Settings.Models;

namespace API.Services;

public class SystemControllerService(IConfiguration configuration, ILogger<SystemControllerService> logger)
{
    private readonly RootSettings _RootSettings = ConfigurationBinderService.Bind<RootSettings>(configuration);
    private readonly DefaultSettings _DefaultSettings = new();
    private readonly ILogger<SystemControllerService> _Logger = logger;
    public async Task<SettingsFetchResponse> GetSettings()
    {
        return new SettingsFetchResponse()
        {
            Database = new DatabaseSettingsFetchResponse()
            {
                ConnectionString = _RootSettings.Database.ConnectionString
            },
            JWT = new JWTSettingsFetchResponse()
            {
                AccessTokenSecret = _RootSettings.JWT.AccessTokenSecret,
                RefreshTokenSecret = _RootSettings.JWT.RefreshTokenSecret,
                TokenTTLMinutes = _RootSettings.JWT.TokenTTLMinutes,
                RenewTokenTTLDays = _RootSettings.JWT.RenewTokenTTLDays,
                Roles = _RootSettings.JWT.Roles,
                Issuer = _RootSettings.JWT.Issuer,
                Audience = _RootSettings.JWT.Audience
            },
            LDAP = new LDAPSettingsFetchResponse()
            {
                Host = _RootSettings.LDAP.Host,
                Port = _RootSettings.LDAP.Port,
                FQDN = _RootSettings.LDAP.FQDN,
                BaseDN = _RootSettings.LDAP.BaseDN,
                SA = _RootSettings.LDAP.SA,
                SAPassword = _RootSettings.LDAP.SAPassword
            },
            Logging = new LoggerSettingsFetchResponse()
            {
                LogLevel = _RootSettings.Logging.LogLevel,
                Console = new ConsoleLoggerSettingsFetchResponse()
                {
                    IsEnabled = _RootSettings.Logging.Console.IsEnabled,
                    LogLevel = _RootSettings.Logging.Console.LogLevel
                },
                FileLogger = new FileLoggerSettingsFetchResponse()
                {
                    IsEnabled = _RootSettings.Logging.FileLogger.IsEnabled,
                    LogLevel = _RootSettings.Logging.FileLogger.LogLevel,
                    Path = _RootSettings.Logging.FileLogger.Path
                },
                DBLogger = new DBLoggerSettingsFetchResponse()
                {
                    IsEnabled = _RootSettings.Logging.DBLogger.IsEnabled,
                    LogLevel = _RootSettings.Logging.DBLogger.LogLevel
                }
            },
            System = new SystemSettingsFetchResponse()
            {
                ListenIP = _RootSettings.System.ListenIP,
                HttpPort = _RootSettings.System.HttpPort,
                HttpsPort = _RootSettings.System.HttpsPort,
                UseSSL = _RootSettings.System.UseSSL,
                PfxCertificatePath = _RootSettings.System.PfxCertificatePath
            }
        };
    }

    public async Task<SettingsSchema> GetSettingsSchema()
    {
        return new SettingsSchema()
        {
            Database = new DatabaseSettingsSchema()
            {
                ConnectionString = new ConnectionStringSchema()
                {
                    Required = IsPropertyRequired(typeof(DatabaseSettings).GetProperty(nameof(_RootSettings.Database.ConnectionString))!),
                    Type = GetSchemaType(_RootSettings.Database.ConnectionString),
                    Description = GetPropertyDescription(typeof(DatabaseSettings).GetProperty(nameof(_RootSettings.Database.ConnectionString))!)
                }
            },
            JWT = new JWTSettingsSchema()
            {
                AccessTokenSecret = new AccessTokenSecretSchema()
                {
                    Required = IsPropertyRequired(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.AccessTokenSecret))!),
                    Type = GetSchemaType(_RootSettings.JWT.AccessTokenSecret),
                    Description = GetPropertyDescription(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.AccessTokenSecret))!)
                },
                RefreshTokenSecret = new RefreshTokenSecretSchema()
                {
                    Required = IsPropertyRequired(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.RefreshTokenSecret))!),
                    Type = GetSchemaType(_RootSettings.JWT.RefreshTokenSecret),
                    Description = GetPropertyDescription(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.RefreshTokenSecret))!)
                },
                TokenTTLMinutes = new TokenTTLMinutesSchema()
                {
                    Required = IsPropertyRequired(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.TokenTTLMinutes))!),
                    Type = GetSchemaType(_RootSettings.JWT.TokenTTLMinutes),
                    Description = GetPropertyDescription(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.TokenTTLMinutes))!),
                    DefaultValue = GetDefaultValue(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.TokenTTLMinutes))!, _DefaultSettings.JWT)
                },
                RenewTokenTTLDays = new RenewTokenTTLDaysSchema()
                {
                    Required = IsPropertyRequired(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.RenewTokenTTLDays))!),
                    Type = GetSchemaType(_RootSettings.JWT.RenewTokenTTLDays),
                    Description = GetPropertyDescription(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.RenewTokenTTLDays))!),
                    DefaultValue = GetDefaultValue(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.RenewTokenTTLDays))!, _DefaultSettings.JWT)
                },
                Roles = new RolesSchema()
                {
                    Required = IsPropertyRequired(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.Roles))!),
                    Type = GetSchemaType(_RootSettings.JWT.Roles),
                    Description = GetPropertyDescription(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.Roles))!),
                    DefaultValue = GetDefaultValue(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.Roles))!, _DefaultSettings.JWT)
                },
                Issuer = new IssuerSchema()
                {
                    Required = IsPropertyRequired(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.Issuer))!),
                    Type = GetSchemaType(_RootSettings.JWT.Issuer),
                    Description = GetPropertyDescription(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.Issuer))!)
                },
                Audience = new AudienceSchema()
                {
                    Required = IsPropertyRequired(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.Audience))!),
                    Type = GetSchemaType(_RootSettings.JWT.Audience),
                    Description = GetPropertyDescription(typeof(JWTSettings).GetProperty(nameof(_RootSettings.JWT.Audience))!)
                }
            },
            LDAP = new LDAPSettingsSchema()
            {
                Host = new HostSchema()
                {
                    Required = IsPropertyRequired(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.Host))!),
                    Type = GetSchemaType(_RootSettings.LDAP.Host),
                    Description = GetPropertyDescription(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.Host))!)
                },
                Port = new PortSchema()
                {
                    Required = IsPropertyRequired(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.Port))!),
                    Type = GetSchemaType(_RootSettings.LDAP.Port),
                    Description = GetPropertyDescription(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.Port))!),
                    DefaultValue = GetDefaultValue(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.Port))!, _DefaultSettings.LDAP)
                },
                FQDN = new FQDNSchema()
                {
                    Required = IsPropertyRequired(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.FQDN))!),
                    Type = GetSchemaType(_RootSettings.LDAP.FQDN),
                    Description = GetPropertyDescription(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.FQDN))!)
                },
                BaseDN = new BaseDNSchema()
                {
                    Required = IsPropertyRequired(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.BaseDN))!),
                    Type = GetSchemaType(_RootSettings.LDAP.BaseDN),
                    Description = GetPropertyDescription(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.BaseDN))!)
                },
                SAUsername = new SAUsernameSchema()
                {
                    Required = IsPropertyRequired(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.SA))!),
                    Type = GetSchemaType(_RootSettings.LDAP.SA),
                    Description = GetPropertyDescription(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.SA))!)
                },
                SAPassword = new SAPasswordSchema()
                {
                    Required = IsPropertyRequired(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.SAPassword))!),
                    Type = GetSchemaType(_RootSettings.LDAP.SAPassword),
                    Description = GetPropertyDescription(typeof(LDAPSettings).GetProperty(nameof(_RootSettings.LDAP.SAPassword))!)
                }
            },
            Logging = new LoggerSettingsSchema()
            {
                LogLevel = new SettingsLogLevelSchema()
                {
                    Required = IsPropertyRequired(typeof(LoggerSettings).GetProperty(nameof(_RootSettings.Logging.LogLevel))!),
                    Type = GetSchemaType(_RootSettings.Logging.LogLevel),
                    Description = GetPropertyDescription(typeof(LoggerSettings).GetProperty(nameof(_RootSettings.Logging.LogLevel))!)
                },
                ConsoleLogger = new ConsoleLoggerSettingsSchema()
                {
                    IsEnabled = new SettingsIsEnabledSchema()
                    {
                        Required = IsPropertyRequired(typeof(ConsoleLoggerSettings).GetProperty(nameof(_RootSettings.Logging.Console.IsEnabled))!),
                        Type = GetSchemaType(_RootSettings.Logging.Console.IsEnabled),
                        Description = GetPropertyDescription(typeof(ConsoleLoggerSettings).GetProperty(nameof(_RootSettings.Logging.Console.IsEnabled))!)
                    },
                    LogLevel = new SettingsLogLevelSchema()
                    {
                        Required = IsPropertyRequired(typeof(ConsoleLoggerSettings).GetProperty(nameof(_RootSettings.Logging.Console.LogLevel))!),
                        Type = GetSchemaType(_RootSettings.Logging.Console.LogLevel),
                        Description = GetPropertyDescription(typeof(ConsoleLoggerSettings).GetProperty(nameof(_RootSettings.Logging.Console.LogLevel))!)
                    }
                },
                FileLogger = new FileLoggerSettingsSchema()
                {
                    IsEnabled = new SettingsIsEnabledSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.IsEnabled))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.IsEnabled),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.IsEnabled))!)
                    },
                    LogLevel = new SettingsLogLevelSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.LogLevel))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.LogLevel),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.LogLevel))!)
                    },
                    Path = new PathSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.Path))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.Path),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.Path))!)
                    }
                },
                DBLogger = new DBLoggerSettingsSchema()
                {
                    IsEnabled = new SettingsIsEnabledSchema()
                    {
                        Required = IsPropertyRequired(typeof(DBLoggerSettings).GetProperty(nameof(_RootSettings.Logging.DBLogger.IsEnabled))!),
                        Type = GetSchemaType(_RootSettings.Logging.DBLogger.IsEnabled),
                        Description = GetPropertyDescription(typeof(DBLoggerSettings).GetProperty(nameof(_RootSettings.Logging.DBLogger.IsEnabled))!)
                    },
                    LogLevel = new SettingsLogLevelSchema()
                    {
                        Required = IsPropertyRequired(typeof(DBLoggerSettings).GetProperty(nameof(_RootSettings.Logging.DBLogger.LogLevel))!),
                        Type = GetSchemaType(_RootSettings.Logging.DBLogger.LogLevel),
                        Description = GetPropertyDescription(typeof(DBLoggerSettings).GetProperty(nameof(_RootSettings.Logging.DBLogger.LogLevel))!)
                    },
                },
            },
            System = new SystemSettingsSchema()
            {
                ListenIP = new ListenIPSchema()
                {
                    Required = IsPropertyRequired(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.ListenIP))!),
                    Type = GetSchemaType(_RootSettings.System.ListenIP),
                    Description = GetPropertyDescription(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.ListenIP))!)
                },
                HttpPort = new HttpPortSchema()
                {
                    Required = IsPropertyRequired(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.HttpPort))!),
                    Type = GetSchemaType(_RootSettings.System.HttpPort),
                    Description = GetPropertyDescription(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.HttpPort))!)
                },
                HttpsPort = new HttpsPortSchema()
                {
                    Required = IsPropertyRequired(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.HttpsPort))!),
                    Type = GetSchemaType(_RootSettings.System.HttpsPort),
                    Description = GetPropertyDescription(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.HttpsPort))!)
                },
                UseSSL = new UseSSLSchema()
                {
                    Required = IsPropertyRequired(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.UseSSL))!),
                    Type = GetSchemaType(_RootSettings.System.UseSSL),
                    Description = GetPropertyDescription(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.UseSSL))!)
                },
                PfxCertificatePath = new PfxCertificatePathSchema()
                {
                    Required = IsPropertyRequired(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.PfxCertificatePath))!),
                    Type = GetSchemaType(_RootSettings.System.PfxCertificatePath),
                    Description = GetPropertyDescription(typeof(SystemSettings).GetProperty(nameof(_RootSettings.System.PfxCertificatePath))!)
                }
            }
        };
    }

    public async Task<bool> UpdateSettings(RootSettings rootSettings)
    {
        try
        {
            string configPath = Path.Combine("config.json");

            JsonSerializerOptions serializerOptions = new()
            {
                WriteIndented = true,
            };

            string jsonString = JsonSerializer.Serialize(rootSettings, serializerOptions);
            await File.WriteAllTextAsync(configPath, jsonString);

            return true;

        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Failed to update settings: {Message}", ex.Message);
            return false;
        }
    }

    private static string GetSchemaType(object value)
    {
        Type t = value.GetType();

        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            return "Dictionary";
        }
        return t.Name ?? "String";
    }

    private static bool IsPropertyRequired(PropertyInfo property)
    {
        // Check for Required attribute
        if (property.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.RequiredAttribute), false).Length != 0)
            return true;

        // Check for nullable reference types (C# 8+ nullable context)
        var nullabilityInfo = new NullabilityInfoContext().Create(property);
        if (nullabilityInfo.WriteState == NullabilityState.NotNull && !property.PropertyType.IsValueType)
            return true;

        // Value types are generally required unless they're nullable
        if (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null)
            return true;

        return false;
    }

    private static object GetDefaultValue(PropertyInfo property, object? defaultInstance)
    {
        if (defaultInstance == null) return "";

        try
        {
            // Verify that the property belongs to the instance type or its base types
            if (!property.DeclaringType!.IsAssignableFrom(defaultInstance.GetType()))
            {
                // Try to find a property with the same name on the instance type
                var instanceProperty = defaultInstance.GetType().GetProperty(property.Name);
                if (instanceProperty != null)
                {
                    property = instanceProperty;
                }
                else
                {
                    return "";
                }
            }

            var value = property.GetValue(defaultInstance);

            // Handle special cases
            if (value is string str)
                return str; // Return the string as-is, including empty strings

            if (value?.GetType().IsValueType == true)
            {
                return value; // Return value types as-is
            }

            // Handle collections
            if (value is System.Collections.ICollection collection)
            {
                return collection.Count > 0 ? value : Array.Empty<object>();
            }

            return value ?? "";
        }
        catch (Exception ex)
        {
            // Log the exception details for debugging
            Console.WriteLine($"GetDefaultValue failed for property {property.Name} on type {defaultInstance.GetType().Name}: {ex.Message}");
            return "";
        }
    }

    private static string GetPropertyDescription(PropertyInfo property)
    {
        var descriptionAttribute = property.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
        if (descriptionAttribute != null)
        {
            return descriptionAttribute.Description;
        }
        return "";
    }
}
