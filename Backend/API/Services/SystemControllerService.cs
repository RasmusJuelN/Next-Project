using API.DTO.Responses.Settings.SettingsSchema.Bases;

namespace API.Services;

public class SystemControllerService(IConfiguration configuration, ILogger<SystemControllerService> logger, IHostApplicationLifetime hostApplicationLifetime)
{
    private readonly RootSettings _RootSettings = ConfigurationBinderService.Bind<RootSettings>(configuration);
    private readonly RootSettings _DefaultSettings = new();
    private readonly ILogger<SystemControllerService> _Logger = logger;
    private readonly JsonSerializerOptions _SerializerOptions = JsonSerializerUtility.ConfigureJsonSerializerSettings();
    private readonly IHostApplicationLifetime _HostApplicationLifetime = hostApplicationLifetime;

    public async Task<bool> StopServer()
    {
        try
        {
            _HostApplicationLifetime.StopApplication();
            return true;
        }
        catch (Exception e)
        {
            _Logger.LogError(e, "Failed to stop application: {Message}", e.Message);
            return false;
        }
    }

    public async Task<FileResult> ExportSettings()
    {
        string jsonString = JsonSerializer.Serialize(_RootSettings, _SerializerOptions);
        var fileBytes = System.Text.Encoding.UTF8.GetBytes(jsonString);
        var contentType = "application/json";
        var currentDate = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var fileName = $"settings_export_{currentDate}.json";

        return new FileContentResult(fileBytes, contentType)
        {
            FileDownloadName = fileName
        };
    }

    public async Task<bool> ImportSettings(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            string jsonString = await reader.ReadToEndAsync();

            var importedSettings = JsonSerializer.Deserialize<UpdateSettingsRequest>(jsonString, _SerializerOptions);
            if (importedSettings == null)
            {
                _Logger.LogError("Imported settings are null.");
                return false;
            }

            string configPath = "config.json";
            string serializedSettings = JsonSerializer.Serialize(importedSettings, _SerializerOptions);
            await File.WriteAllTextAsync(configPath, serializedSettings);

            return true;
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Failed to import settings: {Message}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Retrieves a log file from the logging directory and returns it as a downloadable file.
    /// </summary>
    /// <param name="filename">The name of the log file to retrieve.</param>
    /// <returns>A <see cref="FileResult"/> containing the log file content as a downloadable file with plain text content type.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified log file does not exist in the logging directory.</exception>
    /// <remarks>
    /// The method constructs the full file path by combining the logging directory path from configuration 
    /// with the provided filename. The returned file will have a content type of "text/plain" and will be 
    /// named according to the original filename parameter.
    /// </remarks>
    public async Task<FileResult> GetLogFile(string filename)
    {
        var logFilePath = Path.Combine(Path.GetDirectoryName(_RootSettings.Logging.FileLogger.Path)!, filename);

        if (!File.Exists(logFilePath))
        {
            throw new FileNotFoundException("Log file not found.", logFilePath);
        }

        var fileBytes = await File.ReadAllBytesAsync(logFilePath);
        var contentType = "text/plain";

        return new FileContentResult(fileBytes, contentType)
        {
            FileDownloadName = filename
        };
    }

    /// <summary>
    /// Retrieves a list of log file names from the configured log directory.
    /// </summary>
    /// <returns>
    /// A list of strings containing the names of all files found in the log directory.
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">
    /// Thrown when the log directory path is null or the directory does not exist.
    /// </exception>
    public List<string> GetLogFileNames()
    {
        var logDirectory = Path.GetDirectoryName(_RootSettings.Logging.FileLogger.Path);

        if (logDirectory == null || !Directory.Exists(logDirectory))
        {
            throw new DirectoryNotFoundException("Log directory not found.");
        }

        var logFiles = Directory.GetFiles(logDirectory);
        return logFiles.Select(Path.GetFileName).Where(name => !string.IsNullOrEmpty(name)).ToList()!;
    }

    /// <summary>
    /// Retrieves the current system settings configuration.
    /// </summary>
    /// <returns>
    /// A <see cref="SettingsFetchResponse"/> containing all system configuration settings.
    /// </returns>
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
                    Path = _RootSettings.Logging.FileLogger.Path,
                    RollingInterval = _RootSettings.Logging.FileLogger.RollingInterval,
                    RollOnFileSizeLimit = _RootSettings.Logging.FileLogger.RollOnFileSizeLimit,
                    FileSizeLimitBytes = _RootSettings.Logging.FileLogger.FileSizeLimitBytes,
                    RetainedFileCountLimit = _RootSettings.Logging.FileLogger.RetainedFileCountLimit,
                    Shared = _RootSettings.Logging.FileLogger.Shared
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

    /// <summary>
    /// Retrieves the settings schema definition for the application configuration.
    /// This method builds a comprehensive schema that describes all available settings,
    /// their types, requirements, descriptions, and default values across different
    /// configuration sections including Database, JWT, LDAP, Logging, and System settings.
    /// </summary>
    /// <returns>
    /// A <see cref="SettingsSchema"/> object containing the complete configuration schema
    /// with metadata for each setting including whether it's required, its data type,
    /// description, and default value where applicable.
    /// </returns>
    /// <remarks>
    /// The schema is dynamically generated using reflection to analyze the properties
    /// of the settings classes and their attributes. This ensures the schema remains
    /// synchronized with the actual settings model definitions.
    /// </remarks>
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
                    },
                    RollingInterval = new RollingIntervalSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RollingInterval))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.RollingInterval),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RollingInterval))!),
                        DefaultValue = GetDefaultValue(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RollingInterval))!, _DefaultSettings.Logging.FileLogger)
                    },
                    RollOnFileSizeLimit = new RollOnFileSizeLimitSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RollOnFileSizeLimit))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.RollOnFileSizeLimit),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RollOnFileSizeLimit))!),
                        DefaultValue = GetDefaultValue(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RollOnFileSizeLimit))!, _DefaultSettings.Logging.FileLogger)
                    },
                    FileSizeLimitBytes = new FileSizeLimitBytesSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.FileSizeLimitBytes))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.FileSizeLimitBytes),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.FileSizeLimitBytes))!),
                        DefaultValue = GetDefaultValue(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.FileSizeLimitBytes))!, _DefaultSettings.Logging.FileLogger)
                    },
                    RetainedFileCountLimit = new RetainedFileCountLimitSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RetainedFileCountLimit))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.RetainedFileCountLimit),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RetainedFileCountLimit))!),
                        DefaultValue = GetDefaultValue(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.RetainedFileCountLimit))!, _DefaultSettings.Logging.FileLogger)
                    },
                    Shared = new SharedSchema()
                    {
                        Required = IsPropertyRequired(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.Shared))!),
                        Type = GetSchemaType(_RootSettings.Logging.FileLogger.Shared),
                        Description = GetPropertyDescription(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.Shared))!),
                        DefaultValue = GetDefaultValue(typeof(FileLoggerSettings).GetProperty(nameof(_RootSettings.Logging.FileLogger.Shared))!, _DefaultSettings.Logging.FileLogger)
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

    /// <summary>
    /// Updates the application settings by serializing the provided settings object to a JSON configuration file.
    /// </summary>
    /// <param name="rootSettings">The settings object containing the updated configuration values to be persisted.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value:
    /// <c>true</c> if the settings were successfully updated and saved to the configuration file;
    /// <c>false</c> if an error occurred during the update process.
    /// </returns>
    /// <remarks>
    /// This method writes the serialized settings to a "config.json" file in the application's root directory.
    /// Any exceptions that occur during the serialization or file writing process are logged and the method returns false.
    /// </remarks>
    public async Task<bool> UpdateSettings(UpdateSettingsRequest rootSettings)
    {
        try
        {
            rootSettings.Version = _RootSettings.Version;
            string configPath = "config.json";

            string jsonString = JsonSerializer.Serialize(rootSettings, _SerializerOptions);
            await File.WriteAllTextAsync(configPath, jsonString);

            return true;

        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Failed to update settings: {Message}", ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Asynchronously patches the system settings by merging the provided settings with existing root settings
    /// and persisting the merged configuration to a JSON file.
    /// </summary>
    /// <param name="rootSettings">The patch settings request containing the settings to be merged with existing configuration.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a boolean value:
    /// true if the settings were successfully patched and saved; otherwise, false if an error occurred.
    /// </returns>
    /// <remarks>
    /// This method merges the provided settings with the current root settings, serializes the result to JSON,
    /// and writes it to the "config.json" file. Any exceptions during the process are logged and the method returns false.
    /// </remarks>
    public async Task<bool> PatchSettings(PatchSettingsRequest rootSettings)
    {
        try
        {
            PatchSettingsRequest mergedSettings = PatchSettings(_RootSettings, rootSettings);

            string configPath = "config.json";

            string jsonString = JsonSerializer.Serialize(mergedSettings, _SerializerOptions);
            await File.WriteAllTextAsync(configPath, jsonString);

            return true;
        }
        catch (Exception ex)
        {
            _Logger.LogError(ex, "Failed to patch settings: {Message}", ex.Message);
            return false;
        }
    }

    private static PatchSettingsRequest PatchSettings(RootSettings current, PatchSettingsRequest updates)
    {
        var result = new PatchSettingsRequest
        {
            Version = current.Version,
            Database = new DatabasePatchRequest
            {
                ConnectionString = updates.Database?.ConnectionString ?? current.Database.ConnectionString
            },
            JWT = new JWTPatchRequest
            {
                AccessTokenSecret = updates.JWT?.AccessTokenSecret ?? current.JWT.AccessTokenSecret,
                RefreshTokenSecret = updates.JWT?.RefreshTokenSecret ?? current.JWT.RefreshTokenSecret,
                TokenTTLMinutes = updates.JWT?.TokenTTLMinutes ?? current.JWT.TokenTTLMinutes,
                RenewTokenTTLDays = updates.JWT?.RenewTokenTTLDays ?? current.JWT.RenewTokenTTLDays,
                Roles = updates.JWT?.Roles ?? current.JWT.Roles,
                Issuer = updates.JWT?.Issuer ?? current.JWT.Issuer,
                Audience = updates.JWT?.Audience ?? current.JWT.Audience
            },
            LDAP = new LDAPPatchRequest
            {
                Host = updates.LDAP?.Host ?? current.LDAP.Host,
                Port = updates.LDAP?.Port ?? current.LDAP.Port,
                FQDN = updates.LDAP?.FQDN ?? current.LDAP.FQDN,
                BaseDN = updates.LDAP?.BaseDN ?? current.LDAP.BaseDN,
                SA = updates.LDAP?.SA ?? current.LDAP.SA,
                SAPassword = updates.LDAP?.SAPassword ?? current.LDAP.SAPassword
            },
            Logging = new LoggerPatchRequest
            {
                LogLevel = updates.Logging?.LogLevel ?? current.Logging.LogLevel,
                Console = new ConsoleLoggerPatchRequest
                {
                    IsEnabled = updates.Logging?.Console?.IsEnabled ?? current.Logging.Console.IsEnabled,
                    LogLevel = updates.Logging?.Console?.LogLevel ?? current.Logging.Console.LogLevel
                },
                FileLogger = new FileLoggerPatchRequest
                {
                    IsEnabled = updates.Logging?.FileLogger?.IsEnabled ?? current.Logging.FileLogger.IsEnabled,
                    LogLevel = updates.Logging?.FileLogger?.LogLevel ?? current.Logging.FileLogger.LogLevel,
                    Path = updates.Logging?.FileLogger?.Path ?? current.Logging.FileLogger.Path
                },
                DBLogger = new DBLoggerPatchRequest
                {
                    IsEnabled = updates.Logging?.DBLogger?.IsEnabled ?? current.Logging.DBLogger.IsEnabled,
                    LogLevel = updates.Logging?.DBLogger?.LogLevel ?? current.Logging.DBLogger.LogLevel
                }
            },
            System = new SystemPatchRequest
            {
                ListenIP = updates.System?.ListenIP ?? current.System.ListenIP,
                HttpPort = updates.System?.HttpPort ?? current.System.HttpPort,
                HttpsPort = updates.System?.HttpsPort ?? current.System.HttpsPort,
                UseSSL = updates.System?.UseSSL ?? current.System.UseSSL,
                PfxCertificatePath = updates.System?.PfxCertificatePath ?? current.System.PfxCertificatePath
            }
        };

        return result;
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
