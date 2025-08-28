# Custom Attributes

Custom attributes in the NextQuestionnaire Backend provide metadata that can be used for reflection, mapping, and configuration purposes. This guide explains how to use and create custom attributes in the project.

## What are Custom Attributes?

Attributes are a form of metadata that can be applied to classes, methods, properties, fields, and other code elements. They provide additional information that can be accessed at runtime through reflection or used by frameworks and tools for various purposes.

## Existing Custom Attributes

### AuthenticationMapping Attribute

The `AuthenticationMapping` attribute is used to map fields or properties to named authentication entries.

#### Purpose
- Maps target fields or properties to authentication-related entries
- Used for configuration values, claims, or context items
- Enables dynamic property mapping in authentication scenarios

#### Usage

```csharp
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AuthenticationMapping(string entryName) : Attribute
{
    public string EntryName = entryName;
}
```

#### Example Implementation

```csharp
public class UserProfile
{
    [AuthenticationMapping("user_id")]
    public Guid UserId { get; set; }
    
    [AuthenticationMapping("username")]
    public string UserName { get; set; }
    
    [AuthenticationMapping("email")]
    public string Email { get; set; }
    
    [AuthenticationMapping("role")]
    public string Role { get; set; }
}
```

#### Using the Attribute

```csharp
public class AuthenticationMapper
{
    public void MapProperties<T>(T target, Dictionary<string, object> authData)
    {
        var type = typeof(T);
        var properties = type.GetProperties();
        
        foreach (var property in properties)
        {
            var attribute = property.GetCustomAttribute<AuthenticationMapping>();
            if (attribute != null && authData.ContainsKey(attribute.EntryName))
            {
                var value = authData[attribute.EntryName];
                property.SetValue(target, Convert.ChangeType(value, property.PropertyType));
            }
        }
    }
}

// Usage example
var userProfile = new UserProfile();
var authData = new Dictionary<string, object>
{
    ["user_id"] = Guid.NewGuid(),
    ["username"] = "john.doe",
    ["email"] = "john.doe@example.com",
    ["role"] = "Student"
};

var mapper = new AuthenticationMapper();
mapper.MapProperties(userProfile, authData);
```

## Creating Custom Attributes

### Basic Attribute Structure

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomAttribute : Attribute
{
    public string Value { get; }
    
    public CustomAttribute(string value)
    {
        Value = value;
    }
}
```

### Example: API Version Attribute

```csharp
namespace API.Attributes;

/// <summary>
/// Specifies the API version for a controller or action method.
/// </summary>
/// <param name="version">The API version string (e.g., "1.0", "2.0").</param>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiVersionAttribute(string version) : Attribute
{
    /// <summary>
    /// Gets the API version string.
    /// </summary>
    public string Version { get; } = version;
    
    /// <summary>
    /// Gets or sets a value indicating whether this version is deprecated.
    /// </summary>
    public bool IsDeprecated { get; set; } = false;
}
```

#### Usage

```csharp
[ApiVersion("1.0")]
public class UserController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        // Implementation
    }
    
    [HttpGet("detailed")]
    [ApiVersion("2.0")]
    public async Task<IActionResult> GetUsersDetailed()
    {
        // Implementation
    }
    
    [HttpGet("legacy")]
    [ApiVersion("1.0", IsDeprecated = true)]
    public async Task<IActionResult> GetUsersLegacy()
    {
        // Implementation
    }
}
```

### Example: Caching Attribute

```csharp
namespace API.Attributes;

/// <summary>
/// Specifies caching behavior for controller actions.
/// </summary>
/// <param name="durationSeconds">The cache duration in seconds.</param>
[AttributeUsage(AttributeTargets.Method)]
public class CacheAttribute(int durationSeconds) : Attribute
{
    /// <summary>
    /// Gets the cache duration in seconds.
    /// </summary>
    public int DurationSeconds { get; } = durationSeconds;
    
    /// <summary>
    /// Gets or sets the cache key prefix.
    /// </summary>
    public string? KeyPrefix { get; set; }
    
    /// <summary>
    /// Gets or sets whether the cache varies by user.
    /// </summary>
    public bool VaryByUser { get; set; } = false;
}
```

#### Usage

```csharp
public class ProductController : ControllerBase
{
    [HttpGet]
    [Cache(300, KeyPrefix = "products", VaryByUser = false)] // 5 minutes
    public async Task<IActionResult> GetProducts()
    {
        // Implementation
    }
    
    [HttpGet("user-specific")]
    [Cache(60, KeyPrefix = "user-products", VaryByUser = true)] // 1 minute
    public async Task<IActionResult> GetUserProducts()
    {
        // Implementation
    }
}
```

## Reading Attributes at Runtime

### Using Reflection

```csharp
public class AttributeReader
{
    public static T? GetAttribute<T>(Type type) where T : Attribute
    {
        return type.GetCustomAttribute<T>();
    }
    
    public static T? GetAttribute<T>(PropertyInfo property) where T : Attribute
    {
        return property.GetCustomAttribute<T>();
    }
    
    public static T? GetAttribute<T>(MethodInfo method) where T : Attribute
    {
        return method.GetCustomAttribute<T>();
    }
    
    public static IEnumerable<T> GetAttributes<T>(Type type) where T : Attribute
    {
        return type.GetCustomAttributes<T>();
    }
}
```

### Example: Processing API Version Attributes

```csharp
public class ApiVersionProcessor
{
    public void ProcessController(Type controllerType)
    {
        var classVersion = AttributeReader.GetAttribute<ApiVersionAttribute>(controllerType);
        Console.WriteLine($"Controller {controllerType.Name} version: {classVersion?.Version ?? "Unknown"}");
        
        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (var method in methods)
        {
            var methodVersion = AttributeReader.GetAttribute<ApiVersionAttribute>(method);
            if (methodVersion != null)
            {
                Console.WriteLine($"  Method {method.Name} version: {methodVersion.Version}");
                if (methodVersion.IsDeprecated)
                {
                    Console.WriteLine($"    WARNING: Method {method.Name} is deprecated!");
                }
            }
        }
    }
}
```

## Best Practices

### 1. Use Descriptive Names
```csharp
// ✅ Good - Clear purpose
[ValidationRequired]
[CacheFor(300)]
[ApiVersion("1.0")]

// ❌ Bad - Unclear purpose
[Required]
[Cache]
[V1]
```

### 2. Specify Appropriate Targets
```csharp
// ✅ Good - Specific targets
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ValidationAttribute : Attribute { }

// ❌ Bad - Too broad
[AttributeUsage(AttributeTargets.All)]
public class ValidationAttribute : Attribute { }
```

### 3. Provide Comprehensive Documentation
```csharp
/// <summary>
/// Specifies validation rules for a property or field.
/// </summary>
/// <param name="pattern">The regex pattern for validation.</param>
/// <param name="errorMessage">The error message to display on validation failure.</param>
/// <remarks>
/// This attribute is used by the validation framework to automatically
/// validate property values against the specified regex pattern.
/// </remarks>
/// <example>
/// <code>
/// [Validation(@"^\d{3}-\d{2}-\d{4}$", "Invalid SSN format")]
/// public string SocialSecurityNumber { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ValidationAttribute(string pattern, string errorMessage) : Attribute
{
    public string Pattern { get; } = pattern;
    public string ErrorMessage { get; } = errorMessage;
}
```

### 4. Use Properties for Optional Parameters
```csharp
public class ConfigurationAttribute(string key) : Attribute
{
    public string Key { get; } = key;
    
    // Optional properties
    public bool Required { get; set; } = true;
    public string? DefaultValue { get; set; }
    public string? Description { get; set; }
}
```

### 5. Consider Inheritance for Related Attributes
```csharp
// Base attribute
public abstract class ValidationAttribute : Attribute
{
    public string? ErrorMessage { get; set; }
    
    public abstract bool IsValid(object? value);
}

// Specific implementations
public class RequiredAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value != null && !string.IsNullOrWhiteSpace(value.ToString());
    }
}

public class RangeAttribute(int min, int max) : ValidationAttribute
{
    public int Min { get; } = min;
    public int Max { get; } = max;
    
    public override bool IsValid(object? value)
    {
        if (value is int intValue)
            return intValue >= Min && intValue <= Max;
        return false;
    }
}
```

## Common Use Cases

### 1. Configuration Mapping
```csharp
public class DatabaseSettings
{
    [AuthenticationMapping("db_connection_string")]
    public string ConnectionString { get; set; }
    
    [AuthenticationMapping("db_timeout")]
    public int CommandTimeout { get; set; }
}
```

### 2. API Documentation
```csharp
[ApiVersion("1.0")]
[ApiDescription("User management endpoints")]
public class UserController : ControllerBase
{
    [HttpGet]
    [ApiDescription("Retrieves all users with pagination")]
    [Cache(60)]
    public async Task<IActionResult> GetUsers()
    {
        // Implementation
    }
}
```

### 3. Security and Authorization
```csharp
[RequireRole("Admin")]
[AuditLog(Action = "UserCreation")]
public class AdminController : ControllerBase
{
    [HttpPost]
    [ValidateInput]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        // Implementation
    }
}
```

Custom attributes provide a powerful way to add metadata and behavior to your code in a declarative manner. They enable clean separation of concerns and make your code more maintainable and extensible.
