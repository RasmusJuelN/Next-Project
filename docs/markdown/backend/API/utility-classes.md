# Utility Classes

Utility classes in the NextQuestionnaire Backend provide common functionality that can be reused throughout the application. This guide covers the existing utility classes and how to create your own.

## Overview

Utility classes contain static methods and helper functions that perform common tasks such as cryptographic operations, configuration management, and string transformations. They promote code reuse and maintain consistency across the application.

## Existing Utility Classes

### Crypto Class

The `Crypto` class provides cryptographic hashing utilities for string values with configurable encoding.

```csharp
public class Crypto(string value, Encoding? encoding = null)
{
    private readonly string _value = value;
    private readonly Encoding _encoding = encoding ?? Encoding.UTF8;
}
```

#### Key Features

- **SHA-256 Hashing**: Secure hash generation for strings
- **Configurable Encoding**: Support for different text encodings
- **Instance and Static Methods**: Flexible usage patterns

#### Methods

##### Instance Methods

```csharp
// Create instance and hash
var crypto = new Crypto("sensitive-data");
byte[] hash = crypto.ToSha256();
```

##### Static Methods

```csharp
// Hash with specific encoding
byte[] hash1 = Crypto.ToSha256("data", Encoding.UTF8);

// Hash with default UTF-8 encoding
byte[] hash2 = Crypto.ToSha256("data");
```

#### Usage Examples

**Password Hashing:**
```csharp
public class PasswordService
{
    public string HashPassword(string password, string salt)
    {
        var combinedValue = $"{password}{salt}";
        var hashBytes = Crypto.ToSha256(combinedValue);
        return Convert.ToBase64String(hashBytes);
    }
    
    public bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        var computedHash = HashPassword(password, salt);
        return computedHash == hashedPassword;
    }
}
```

**API Key Generation:**
```csharp
public class ApiKeyGenerator
{
    public string GenerateApiKey(string userId, DateTime expiry)
    {
        var keyData = $"{userId}:{expiry:O}:{Guid.NewGuid()}";
        var hashBytes = Crypto.ToSha256(keyData);
        return Convert.ToHexString(hashBytes);
    }
}
```

**File Integrity Verification:**
```csharp
public class FileService
{
    public string CalculateFileHash(string filePath)
    {
        var fileContent = File.ReadAllText(filePath);
        var hashBytes = Crypto.ToSha256(fileContent);
        return Convert.ToHexString(hashBytes);
    }
    
    public bool VerifyFileIntegrity(string filePath, string expectedHash)
    {
        var actualHash = CalculateFileHash(filePath);
        return actualHash.Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
    }
}
```

### SettingsHelper Class

The `SettingsHelper` class manages application settings files, including creation of default settings and checking for file existence.

```csharp
public class SettingsHelper(string settingsFile)
{
    private readonly string _settingsFile = settingsFile;
    private RootSettings _defaultSettings = new();
    private JsonSerializerService _serializer = new();
}
```

#### Key Features

- **Settings File Management**: Check existence and create default files
- **JSON Serialization**: Automatic serialization of settings objects
- **User Interaction**: Prompts for configuration completion

#### Methods

##### SettingsExists()

```csharp
public bool SettingsExists()
{
    return File.Exists(_settingsFile);
}
```

##### CreateDefault()

```csharp
public void CreateDefault()
{
    string json = _serializer.Serialize(_defaultSettings);
    File.WriteAllText(_settingsFile, json);
    
    // Prompt user to configure settings
    Console.WriteLine("Settings file created. Please configure required settings.");
    Console.ReadLine();
}
```

#### Usage Examples

**Application Startup:**
```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var settingsHelper = new SettingsHelper("appsettings.json");
        
        if (!settingsHelper.SettingsExists())
        {
            Console.WriteLine("Settings file not found. Creating default settings...");
            settingsHelper.CreateDefault();
        }
        
        // Continue with application startup
        var app = CreateApplication();
        app.Run();
    }
}
```

**Configuration Validation:**
```csharp
public class ConfigurationService
{
    public void EnsureConfigurationExists(string configPath)
    {
        var settingsHelper = new SettingsHelper(configPath);
        
        if (!settingsHelper.SettingsExists())
        {
            throw new InvalidOperationException(
                $"Configuration file not found at {configPath}. " +
                "Run the application once to generate default settings.");
        }
    }
    
    public void InitializeConfiguration(string configPath)
    {
        var settingsHelper = new SettingsHelper(configPath);
        
        if (!settingsHelper.SettingsExists())
        {
            settingsHelper.CreateDefault();
        }
    }
}
```

### SlugifyParameterTransformer Class

The `SlugifyParameterTransformer` converts PascalCase or camelCase parameter names to kebab-case (slug format) for URLs.

```csharp
public partial class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value == null) return null;
        return MyRegex().Replace(value.ToString()!, "$1-$2").ToLower();
    }

    [GeneratedRegex("([a-z])([A-Z])")]
    private static partial Regex MyRegex();
}
```

#### Key Features

- **URL-Friendly Formatting**: Converts to kebab-case for better URLs
- **Regex-Based Transformation**: Efficient pattern matching
- **Null-Safe**: Handles null input gracefully

#### Usage Examples

**Route Configuration:**
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.ConstraintMap.Add("slugify", typeof(SlugifyParameterTransformer));
        });
        
        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(
                new SlugifyParameterTransformer()));
        });
    }
}
```

**Transformation Examples:**
```csharp
var transformer = new SlugifyParameterTransformer();

// Controller names
transformer.TransformOutbound("UserController"); // "user-controller"
transformer.TransformOutbound("QuestionnaireTemplateController"); // "questionnaire-template-controller"

// Action names
transformer.TransformOutbound("GetUserById"); // "get-user-by-id"
transformer.TransformOutbound("CreateNewQuestionnaire"); // "create-new-questionnaire"
```

**Manual Usage:**
```csharp
public class UrlService
{
    private readonly SlugifyParameterTransformer _transformer = new();
    
    public string CreateFriendlyUrl(string basePath, string identifier)
    {
        var slugifiedIdentifier = _transformer.TransformOutbound(identifier);
        return $"{basePath}/{slugifiedIdentifier}";
    }
}

// Usage
var urlService = new UrlService();
var url = urlService.CreateFriendlyUrl("/api", "QuestionnaireTemplate");
// Result: "/api/questionnaire-template"
```

## Creating Custom Utility Classes

### Example: String Utilities

```csharp
namespace API.Utils;

/// <summary>
/// Provides utility methods for string operations.
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// Truncates a string to the specified maximum length.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum length allowed.</param>
    /// <param name="ellipsis">Whether to add ellipsis (...) when truncating.</param>
    /// <returns>The truncated string.</returns>
    public static string Truncate(string value, int maxLength, bool ellipsis = true)
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value;
            
        var truncated = value.Substring(0, maxLength);
        return ellipsis ? $"{truncated}..." : truncated;
    }
    
    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The camelCase string.</returns>
    public static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsLower(value[0]))
            return value;
            
        return char.ToLowerInvariant(value[0]) + value.Substring(1);
    }
    
    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The PascalCase string.</returns>
    public static string ToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value) || char.IsUpper(value[0]))
            return value;
            
        return char.ToUpperInvariant(value[0]) + value.Substring(1);
    }
    
    /// <summary>
    /// Removes all whitespace from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>The string without whitespace.</returns>
    public static string RemoveWhitespace(string value)
    {
        return Regex.Replace(value ?? string.Empty, @"\s+", string.Empty);
    }
    
    /// <summary>
    /// Masks sensitive information in a string.
    /// </summary>
    /// <param name="value">The string to mask.</param>
    /// <param name="visibleCharacters">Number of characters to keep visible at the start.</param>
    /// <param name="maskCharacter">The character to use for masking.</param>
    /// <returns>The masked string.</returns>
    public static string MaskSensitiveData(string value, int visibleCharacters = 2, char maskCharacter = '*')
    {
        if (string.IsNullOrEmpty(value) || value.Length <= visibleCharacters)
            return new string(maskCharacter, value?.Length ?? 0);
            
        var visiblePart = value.Substring(0, visibleCharacters);
        var maskedPart = new string(maskCharacter, value.Length - visibleCharacters);
        return visiblePart + maskedPart;
    }
}
```

### Example: Date Utilities

```csharp
namespace API.Utils;

/// <summary>
/// Provides utility methods for date and time operations.
/// </summary>
public static class DateUtils
{
    /// <summary>
    /// Gets the start of the week for the given date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="startOfWeek">The day considered as start of week.</param>
    /// <returns>The start of the week.</returns>
    public static DateTime GetStartOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-1 * diff).Date;
    }
    
    /// <summary>
    /// Gets the end of the week for the given date.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="startOfWeek">The day considered as start of week.</param>
    /// <returns>The end of the week.</returns>
    public static DateTime GetEndOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        return GetStartOfWeek(date, startOfWeek).AddDays(6).Date.AddDays(1).AddTicks(-1);
    }
    
    /// <summary>
    /// Calculates the age based on birthdate.
    /// </summary>
    /// <param name="birthDate">The birth date.</param>
    /// <returns>The age in years.</returns>
    public static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        
        if (birthDate.Date > today.AddYears(-age))
            age--;
            
        return age;
    }
    
    /// <summary>
    /// Checks if a date is a business day (Monday-Friday).
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns>True if it's a business day; otherwise, false.</returns>
    public static bool IsBusinessDay(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
    }
    
    /// <summary>
    /// Gets the next business day after the given date.
    /// </summary>
    /// <param name="date">The starting date.</param>
    /// <returns>The next business day.</returns>
    public static DateTime GetNextBusinessDay(DateTime date)
    {
        do
        {
            date = date.AddDays(1);
        }
        while (!IsBusinessDay(date));
        
        return date;
    }
    
    /// <summary>
    /// Formats a timespan as a human-readable duration.
    /// </summary>
    /// <param name="timeSpan">The timespan to format.</param>
    /// <returns>A human-readable duration string.</returns>
    public static string FormatDuration(TimeSpan timeSpan)
    {
        if (timeSpan.TotalDays >= 1)
            return $"{(int)timeSpan.TotalDays} day(s), {timeSpan.Hours} hour(s)";
        if (timeSpan.TotalHours >= 1)
            return $"{(int)timeSpan.TotalHours} hour(s), {timeSpan.Minutes} minute(s)";
        if (timeSpan.TotalMinutes >= 1)
            return $"{(int)timeSpan.TotalMinutes} minute(s)";
        
        return $"{(int)timeSpan.TotalSeconds} second(s)";
    }
}
```

### Example: Validation Utilities

```csharp
namespace API.Utils;

/// <summary>
/// Provides utility methods for data validation.
/// </summary>
public static class ValidationUtils
{
    /// <summary>
    /// Validates an email address format.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email is valid; otherwise, false.</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
        return emailRegex.IsMatch(email);
    }
    
    /// <summary>
    /// Validates a GUID format.
    /// </summary>
    /// <param name="guidString">The GUID string to validate.</param>
    /// <returns>True if the GUID is valid; otherwise, false.</returns>
    public static bool IsValidGuid(string guidString)
    {
        return Guid.TryParse(guidString, out _);
    }
    
    /// <summary>
    /// Validates a phone number format.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns>True if the phone number is valid; otherwise, false.</returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;
            
        // Remove all non-digit characters
        var digitsOnly = Regex.Replace(phoneNumber, @"[^\d]", "");
        
        // Check if it's between 10-15 digits (international format)
        return digitsOnly.Length >= 10 && digitsOnly.Length <= 15;
    }
    
    /// <summary>
    /// Validates that a string contains only alphanumeric characters.
    /// </summary>
    /// <param name="value">The string to validate.</param>
    /// <returns>True if the string is alphanumeric; otherwise, false.</returns>
    public static bool IsAlphanumeric(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;
            
        return Regex.IsMatch(value, "^[a-zA-Z0-9]+$");
    }
    
    /// <summary>
    /// Validates a URL format.
    /// </summary>
    /// <param name="url">The URL to validate.</param>
    /// <returns>True if the URL is valid; otherwise, false.</returns>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;
            
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
    
    /// <summary>
    /// Validates that a collection is not null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to validate.</param>
    /// <returns>True if the collection is not null or empty; otherwise, false.</returns>
    public static bool IsNotNullOrEmpty<T>(IEnumerable<T>? collection)
    {
        return collection != null && collection.Any();
    }
}
```

### Example: File Utilities

```csharp
namespace API.Utils;

/// <summary>
/// Provides utility methods for file operations.
/// </summary>
public static class FileUtils
{
    /// <summary>
    /// Gets a safe filename by removing invalid characters.
    /// </summary>
    /// <param name="filename">The original filename.</param>
    /// <returns>A safe filename with invalid characters removed.</returns>
    public static string GetSafeFilename(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
            return "unnamed_file";
            
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeFilename = string.Join("_", filename.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        
        return string.IsNullOrWhiteSpace(safeFilename) ? "unnamed_file" : safeFilename;
    }
    
    /// <summary>
    /// Gets the file size in a human-readable format.
    /// </summary>
    /// <param name="bytes">The file size in bytes.</param>
    /// <returns>A human-readable file size string.</returns>
    public static string FormatFileSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        
        double size = bytes;
        int suffixIndex = 0;
        
        while (size >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            size /= 1024;
            suffixIndex++;
        }
        
        return $"{size:N2} {suffixes[suffixIndex]}";
    }
    
    /// <summary>
    /// Ensures a directory exists, creating it if necessary.
    /// </summary>
    /// <param name="directoryPath">The directory path.</param>
    public static void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
    
    /// <summary>
    /// Gets the MIME type for a file based on its extension.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <returns>The MIME type.</returns>
    public static string GetMimeType(string filename)
    {
        var extension = Path.GetExtension(filename)?.ToLowerInvariant();
        
        return extension switch
        {
            ".txt" => "text/plain",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".csv" => "text/csv",
            ".json" => "application/json",
            ".xml" => "application/xml",
            _ => "application/octet-stream"
        };
    }
}
```

## Usage Examples in Services

### Using Multiple Utilities Together

```csharp
public class UserProfileService
{
    public async Task<UserProfile> CreateUserProfile(CreateUserProfileRequest request)
    {
        // Validate input using ValidationUtils
        if (!ValidationUtils.IsValidEmail(request.Email))
        {
            throw new ArgumentException("Invalid email format");
        }
        
        if (!ValidationUtils.IsValidPhoneNumber(request.PhoneNumber))
        {
            throw new ArgumentException("Invalid phone number format");
        }
        
        // Process and clean data using StringUtils
        var displayName = StringUtils.ToCamelCase(request.DisplayName);
        var bio = StringUtils.Truncate(request.Bio, 500, true);
        
        // Generate secure hash for sensitive data using Crypto
        var profileId = Crypto.ToSha256($"{request.Email}:{DateTime.UtcNow:O}");
        
        // Create user profile
        var userProfile = new UserProfile
        {
            Id = new Guid(profileId.Take(16).ToArray()), // Use first 16 bytes as GUID
            Email = request.Email,
            DisplayName = displayName,
            Bio = bio,
            PhoneNumber = request.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.AddAsync(userProfile);
        return userProfile;
    }
}
```

### Configuration Management

```csharp
public class ConfigurationManager
{
    public void InitializeApplication()
    {
        var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        var settingsHelper = new SettingsHelper(settingsPath);
        
        if (!settingsHelper.SettingsExists())
        {
            Console.WriteLine("Configuration file not found. Creating default configuration...");
            settingsHelper.CreateDefault();
        }
        
        // Validate configuration file integrity
        var configHash = Crypto.ToSha256(File.ReadAllText(settingsPath));
        Console.WriteLine($"Configuration hash: {Convert.ToHexString(configHash)}");
    }
}
```

## Best Practices

### 1. Make Utility Classes Static
```csharp
// ✅ Good - Static utility class
public static class StringUtils
{
    public static string ToCamelCase(string value) { }
}

// ❌ Bad - Instance utility class
public class StringUtils
{
    public string ToCamelCase(string value) { } // Requires instantiation
}
```

### 2. Handle Edge Cases
```csharp
// ✅ Good - Handles null and edge cases
public static string Truncate(string value, int maxLength, bool ellipsis = true)
{
    if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
        return value;
    // ... rest of implementation
}

// ❌ Bad - Doesn't handle edge cases
public static string Truncate(string value, int maxLength)
{
    return value.Substring(0, maxLength); // Throws if value is null or too short
}
```

### 3. Use Descriptive Method Names
```csharp
// ✅ Good - Clear purpose
public static bool IsValidEmail(string email)
public static string FormatFileSize(long bytes)
public static DateTime GetNextBusinessDay(DateTime date)

// ❌ Bad - Unclear purpose
public static bool Check(string input)
public static string Format(long number)
public static DateTime GetNext(DateTime date)
```

### 4. Provide Comprehensive Documentation
```csharp
/// <summary>
/// Masks sensitive information in a string by replacing characters with a mask character.
/// </summary>
/// <param name="value">The string to mask.</param>
/// <param name="visibleCharacters">Number of characters to keep visible at the start.</param>
/// <param name="maskCharacter">The character to use for masking. Default is '*'.</param>
/// <returns>The masked string with specified characters visible and the rest masked.</returns>
/// <example>
/// <code>
/// var masked = StringUtils.MaskSensitiveData("12345678", 2, '*');
/// // Result: "12******"
/// </code>
/// </example>
public static string MaskSensitiveData(string value, int visibleCharacters = 2, char maskCharacter = '*')
```

Utility classes provide essential functionality that can be reused throughout your application, promoting code consistency and reducing duplication. They should be well-tested, handle edge cases gracefully, and provide clear, predictable behavior.
