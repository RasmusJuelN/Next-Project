# LINQ-to-LDAP Implementation

The LINQ-to-LDAP implementation provides a powerful abstraction layer that allows developers to use familiar LINQ syntax for querying LDAP/Active Directory data sources. This system seamlessly integrates with the authentication infrastructure and provides type-safe, expressive querying capabilities.

## Overview

Traditional LDAP queries require manual filter construction and result parsing, which can be error-prone and difficult to maintain. The LINQ-to-LDAP implementation solves this by:

- **Type Safety**: Compile-time checking of property access and query structure
- **Familiar Syntax**: Uses standard LINQ operators that developers already know
- **Automatic Translation**: Converts LINQ expressions to proper LDAP filter syntax
- **Seamless Integration**: Works directly with existing authentication and user management systems

## Architecture

The implementation consists of three core components that work together to provide seamless LDAP querying:

### Core Components

```
LINQ Expression → LdapQueryable<T> → LdapQueryProvider → LdapQueryTranslator → LDAP Filter → Active Directory
```

#### LdapQueryable\<T\>
The queryable collection that implements `IQueryable<T>` and serves as the entry point for LINQ operations. It provides:
- Standard LINQ interface compatibility
- Integration with Entity Framework-style query patterns  
- Deferred execution semantics

#### LdapQueryProvider
The query provider handles expression translation and execution by:
- Managing the bridge between LINQ expressions and LDAP operations
- Coordinating query execution through the authentication service
- Handling both single-result and collection queries appropriately

#### LdapQueryTranslator
The expression visitor that converts LINQ expressions to LDAP filter strings using:
- Expression tree traversal with the Visitor pattern
- Support for complex logical operations and comparisons
- Automatic attribute mapping using custom attributes

## Supported Operations

### LINQ Methods
The implementation supports the most common LINQ query operations:

- **`Where()`** - Filtering with complex predicates supporting multiple conditions
- **`FirstOrDefault()`** - Single result retrieval with null safety
- **Standard enumeration** - `ToList()`, `ToArray()`, and other collection methods

### Expression Types

#### Basic Comparisons
```csharp
// Equality comparison
user => user.Name == "john"
// Generates: (Name=john)

// String contains operation  
user => user.Name.Contains("joh")
// Generates: (Name=*joh*)

// Null checks
user => user.Email != null
// Generates: (Email=*)
```

#### Logical Operations
```csharp
// Logical AND
user => user.Name == "john" && user.Email != null
// Generates: (&(Name=john)(Email=*))

// Logical OR  
user => user.Name == "john" || user.Name == "jane"
// Generates: (|(Name=john)(Name=jane))

// Complex nested conditions
user => (user.Name == "john" || user.Name == "jane") && user.Active == true
// Generates: (&(|(Name=john)(Name=jane))(Active=TRUE))
```

### Attribute Mapping

Properties decorated with `[AuthenticationMapping("ldapAttribute")]` are automatically mapped to their corresponding LDAP attribute names during query translation:

```csharp
public class BasicUserInfoWithObjectGuidLinq
{
    [AuthenticationMapping("sAMAccountName")]
    public string Username { get; set; }
    
    [AuthenticationMapping("displayName")]  
    public string Name { get; set; }
    
    [AuthenticationMapping("mail")]
    public string Email { get; set; }
}
```

## Usage Examples

### Basic User Search
```csharp
// Find users by username pattern
var users = authBridge.Query<BasicUserInfoWithObjectGuidLinq>()
    .Where(u => u.Username.Contains("john"))
    .ToList();
```

### Single User Lookup
```csharp
// Find specific user with fallback
var user = authBridge.Query<BasicUserInfoWithObjectGuidLinq>()
    .Where(u => u.Username == "john.doe" && u.Name.Contains("John"))
    .FirstOrDefault();

if (user != null)
{
    Console.WriteLine($"Found user: {user.Name}");
}
```

### Dynamic Search Terms
```csharp
// Using variables in queries
var searchTerm = "admin";
var admins = authBridge.Query<BasicUserInfoWithObjectGuidLinq>()
    .Where(u => u.MemberOf.Contains(searchTerm))
    .ToList();
```

### Complex Filtering
```csharp
// Multiple conditions with logical operators
var activeUsers = authBridge.Query<BasicUserInfoWithObjectGuidLinq>()
    .Where(u => u.Email != null && 
               (u.Name.Contains("Admin") || u.Name.Contains("Manager")) &&
               u.Username != "system")
    .ToList();
```

## Implementation Details

### Expression Tree Processing

The system processes LINQ expression trees by implementing the Visitor pattern through `LdapQueryTranslator`. Each expression node type is handled specifically:

- **Binary expressions** (==, !=, &&, ||) become logical LDAP operations
- **Method calls** (Contains, FirstOrDefault) are translated to appropriate LDAP wildcards or result handling
- **Member access** is mapped through attribute annotations to LDAP property names
- **Constant values** are properly escaped for LDAP filter syntax

### Result Type Handling

The provider automatically detects the expected return type based on the LINQ method chain:

- **Collection methods** (`Where`, `ToList`, `ToArray`) return full result sets as `IEnumerable<T>`
- **Aggregation methods** (`FirstOrDefault`, `Single`) return individual items as `T` or `null`
- **Execution timing** is deferred until enumeration or explicit execution methods are called

### Error Handling and Logging

All components include comprehensive error handling:

- **Expression parsing errors** provide clear messages about unsupported LINQ operations
- **LDAP connection issues** are wrapped in meaningful exceptions with context
- **Query execution failures** include the generated LDAP filter for debugging
- **Performance logging** tracks query execution times and result counts

## Configuration Requirements

The LINQ-to-LDAP system requires proper LDAP configuration in your application settings:

```json
{
  "LdapSettings": {
    "Host": "your-domain-controller.company.com",
    "Port": 389,
    "BaseDN": "DC=company,DC=com", 
    "UseSSL": false,
    "Timeout": 30000
  }
}
```

### Key Configuration Properties

- **BaseDN**: The base distinguished name where user searches will be performed
- **Connection settings**: Proper host, port, and security configuration for your LDAP server
- **Authentication**: Service account credentials if required for your LDAP setup

## Testing and Validation

The implementation includes comprehensive unit tests covering:

### Test Coverage Areas
- **Expression translation accuracy** - Verifying LINQ-to-LDAP filter conversion
- **Attribute mapping functionality** - Testing custom attribute resolution  
- **Complex query scenarios** - Multi-condition and nested logical operations
- **Error handling** - Invalid expressions and connection failure scenarios
- **Performance characteristics** - Query execution timing and resource usage

### Running Tests
```bash
cd Backend/UnitTests
dotnet test --filter "LdapQueryTranslator*" --logger "console;verbosity=minimal"
```

The test suite validates that LINQ expressions are correctly translated to LDAP filter syntax and that the resulting queries execute properly against test scenarios.

## Best Practices

### Query Optimization
- **Use specific conditions**: Avoid overly broad searches that return large result sets
- **Leverage indexing**: Query against indexed LDAP attributes when possible  
- **Limit result sets**: Use `FirstOrDefault()` when you only need a single result

### Error Handling
```csharp
try 
{
    var user = authBridge.Query<BasicUserInfoWithObjectGuidLinq>()
        .Where(u => u.Username == username)
        .FirstOrDefault();
        
    if (user == null)
    {
        // Handle user not found scenario
        return NotFound($"User '{username}' not found");
    }
    
    return Ok(user);
}
catch (LdapException ex)
{
    // Handle LDAP-specific errors
    logger.LogError(ex, "LDAP query failed for username: {Username}", username);
    return StatusCode(500, "Authentication service unavailable");
}
```

### Performance Considerations
- **Connection pooling**: The system reuses LDAP connections efficiently
- **Query caching**: Consider caching frequently accessed user data
- **Batch operations**: When possible, structure queries to minimize round trips

## Integration with Authentication

The LINQ-to-LDAP system integrates seamlessly with the existing authentication infrastructure:

```csharp
public class AuthController : ControllerBase
{
    private readonly ActiveDirectoryAuthenticationBridge authBridge;
    
    [HttpGet("search")]
    public async Task<ActionResult<List<BasicUserInfoWithObjectGuid>>> SearchUsers(
        string searchTerm)
    {
        var users = authBridge.Query<BasicUserInfoWithObjectGuidLinq>()
            .Where(u => u.Name.Contains(searchTerm) || u.Username.Contains(searchTerm))
            .ToList()
            .Select(u => new BasicUserInfoWithObjectGuid 
            {
                Username = u.Username,
                Name = u.Name,
                Email = u.Email,
                ObjectGuid = u.ObjectGuid.ToGuid()  
            })
            .ToList();
            
        return Ok(users);
    }
}
```

This powerful abstraction layer makes LDAP querying as intuitive as working with any other data source in .NET, while maintaining the performance and capabilities of the underlying LDAP protocol.
