# Custom Exceptions

Custom exceptions in the NextQuestionnaire Backend provide specific error handling for different types of failures that can occur in the application. This guide explains the existing custom exceptions and how to use them effectively.

## Overview

Custom exceptions provide more specific error information than generic system exceptions, enabling better error handling, debugging, and user experience. The NextQuestionnaire Backend includes three main categories of custom exceptions.

## HTTP Response Exceptions

### HttpResponseException

The `HttpResponseException` is used to throw exceptions that should be directly translated to HTTP responses with specific status codes.

```csharp
public class HttpResponseException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}
```

#### Usage Examples

```csharp
// Bad Request (400)
throw new HttpResponseException(HttpStatusCode.BadRequest, "Invalid input parameters");

// Unauthorized (401)
throw new HttpResponseException(HttpStatusCode.Unauthorized, "Invalid authentication token");

// Forbidden (403)
throw new HttpResponseException(HttpStatusCode.Forbidden, "Insufficient permissions");

// Not Found (404)
throw new HttpResponseException(HttpStatusCode.NotFound, "Resource not found");

// Conflict (409)
throw new HttpResponseException(HttpStatusCode.Conflict, "Resource already exists");
```

#### In Controller Actions

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<BookResponse>> GetBook(Guid id)
{
    try
    {
        var book = await _bookService.GetBookByIdAsync(id);
        return Ok(book);
    }
    catch (HttpResponseException ex)
    {
        return StatusCode((int)ex.StatusCode, ex.Message);
    }
}
```

#### In Service Layer

```csharp
public async Task<User> GetUserById(Guid id)
{
    var user = await _repository.GetByIdAsync(id);
    
    if (user == null)
    {
        throw new HttpResponseException(
            HttpStatusCode.NotFound, 
            $"User with ID {id} was not found"
        );
    }
    
    return user;
}
```

## LDAP Exceptions

The `LDAPException` class contains specific exceptions for LDAP operations, providing detailed error information for authentication and directory service issues.

### LDAPException.ConnectionError

Thrown when there's a connection problem with the LDAP server.

```csharp
public class ConnectionError : Exception
{
    public ConnectionError() { }
    public ConnectionError(string message) : base(message) { }
    public ConnectionError(string message, Exception inner) : base(message, inner) { }
}
```

#### Usage Examples

```csharp
try
{
    _connection.Connect(ldapHost, ldapPort);
}
catch (LdapException ex)
{
    throw new LDAPException.ConnectionError(
        "Failed to connect to LDAP server", ex);
}
```

### LDAPException.InvalidCredentials

Thrown when authentication fails due to invalid username or password.

```csharp
public class InvalidCredentials : Exception
{
    public InvalidCredentials() { }
    public InvalidCredentials(string message) : base(message) { }
    public InvalidCredentials(string message, Exception inner) : base(message, inner) { }
}
```

#### Usage Examples

```csharp
public void Authenticate(string username, string password)
{
    try
    {
        _connection.Bind(username, password);
    }
    catch (LdapAuthenticationException ex)
    {
        throw new LDAPException.InvalidCredentials(
            "Invalid username or password", ex);
    }
}
```

### LDAPException.NotBound

Thrown when attempting LDAP operations without being properly authenticated.

```csharp
public class NotBound : Exception
{
    public NotBound() { }
    public NotBound(string message) : base(message) { }
    public NotBound(string message, Exception inner) : base(message, inner) { }
}
```

#### Usage Examples

```csharp
public List<User> SearchUsers(string searchTerm)
{
    if (!_connection.Bound)
    {
        throw new LDAPException.NotBound(
            "Must authenticate before performing search operations");
    }
    
    // Perform search...
}
```

## SQL Exceptions

The `SQLException` class contains exceptions specific to database operations.

### SQLException.ItemAlreadyExists

Thrown when attempting to create a resource that already exists.

```csharp
public class ItemAlreadyExists : Exception
{
    public ItemAlreadyExists() { }
    public ItemAlreadyExists(string message) : base(message) { }
    public ItemAlreadyExists(string message, Exception inner) : base(message, inner) { }
}
```

#### Usage Examples

```csharp
public async Task<User> CreateUser(CreateUserRequest request)
{
    var existingUser = await _repository.FindByEmailAsync(request.Email);
    
    if (existingUser != null)
    {
        throw new SQLException.ItemAlreadyExists(
            $"User with email {request.Email} already exists");
    }
    
    // Create user...
}
```

### SQLException.ItemNotFound

Thrown when attempting to access a resource that doesn't exist.

```csharp
public class ItemNotFound : Exception
{
    public ItemNotFound() { }
    public ItemNotFound(string message) : base(message) { }
    public ItemNotFound(string message, Exception inner) : base(message, inner) { }
}
```

#### Usage Examples

```csharp
public async Task<QuestionnaireTemplate> GetTemplateById(Guid id)
{
    var template = await _repository.GetByIdAsync(id);
    
    if (template == null)
    {
        throw new SQLException.ItemNotFound(
            $"Questionnaire template with ID {id} was not found");
    }
    
    return template;
}
```

## Exception Handling Patterns

### 1. Service Layer Pattern

```csharp
public class UserService
{
    public async Task<User> AuthenticateUser(string username, string password)
    {
        try
        {
            // Authenticate with LDAP
            _ldapService.Authenticate(username, password);
            
            // Get user from database
            var user = await _unitOfWork.User.GetByUsernameAsync(username);
            
            if (user == null)
            {
                throw new SQLException.ItemNotFound(
                    $"User {username} not found in local database");
            }
            
            return user;
        }
        catch (LDAPException.InvalidCredentials)
        {
            throw new HttpResponseException(
                HttpStatusCode.Unauthorized, 
                "Invalid username or password");
        }
        catch (LDAPException.ConnectionError ex)
        {
            throw new HttpResponseException(
                HttpStatusCode.ServiceUnavailable, 
                "Authentication service temporarily unavailable");
        }
    }
}
```

### 2. Controller Exception Handling

```csharp
[ApiController]
public class QuestionnaireController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuestionnaireTemplate>> CreateTemplate(
        [FromBody] CreateTemplateRequest request)
    {
        try
        {
            var template = await _service.CreateTemplateAsync(request);
            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
        }
        catch (SQLException.ItemAlreadyExists ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (HttpResponseException ex)
        {
            return StatusCode((int)ex.StatusCode, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating template");
            return StatusCode(500, new { message = "An unexpected error occurred" });
        }
    }
}
```

### 3. Global Exception Handling Middleware

```csharp
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var response = ex switch
        {
            HttpResponseException httpEx => new
            {
                statusCode = (int)httpEx.StatusCode,
                message = httpEx.Message
            },
            SQLException.ItemNotFound => new
            {
                statusCode = 404,
                message = ex.Message
            },
            SQLException.ItemAlreadyExists => new
            {
                statusCode = 409,
                message = ex.Message
            },
            LDAPException.InvalidCredentials => new
            {
                statusCode = 401,
                message = "Authentication failed"
            },
            LDAPException.ConnectionError => new
            {
                statusCode = 503,
                message = "Authentication service unavailable"
            },
            _ => new
            {
                statusCode = 500,
                message = "An unexpected error occurred"
            }
        };

        context.Response.StatusCode = response.statusCode;
        
        _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
        
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
```

## Creating Custom Exceptions

### Basic Custom Exception

```csharp
namespace API.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public class BusinessRuleException : Exception
{
    public string RuleName { get; }
    
    public BusinessRuleException(string ruleName) : base($"Business rule violated: {ruleName}")
    {
        RuleName = ruleName;
    }
    
    public BusinessRuleException(string ruleName, string message) : base(message)
    {
        RuleName = ruleName;
    }
    
    public BusinessRuleException(string ruleName, string message, Exception innerException) 
        : base(message, innerException)
    {
        RuleName = ruleName;
    }
}
```

### Exception with Additional Data

```csharp
namespace API.Exceptions;

/// <summary>
/// Exception thrown when validation fails with detailed error information.
/// </summary>
public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }
    
    public ValidationException(Dictionary<string, string[]> errors) 
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
    
    public ValidationException(string field, string error) 
        : base($"Validation failed for {field}: {error}")
    {
        Errors = new Dictionary<string, string[]>
        {
            [field] = new[] { error }
        };
    }
}
```

## Best Practices

### 1. Use Specific Exception Types

```csharp
// ✅ Good - Specific exception types
throw new SQLException.ItemNotFound("User not found");
throw new LDAPException.InvalidCredentials("Authentication failed");

// ❌ Bad - Generic exceptions
throw new Exception("Something went wrong");
throw new InvalidOperationException("Error occurred");
```

### 2. Provide Meaningful Messages

```csharp
// ✅ Good - Descriptive messages
throw new SQLException.ItemNotFound($"Questionnaire template with ID {id} was not found");
throw new LDAPException.ConnectionError($"Failed to connect to LDAP server at {host}:{port}");

// ❌ Bad - Vague messages
throw new SQLException.ItemNotFound("Not found");
throw new LDAPException.ConnectionError("Connection failed");
```

### 3. Preserve Inner Exceptions

```csharp
// ✅ Good - Preserve inner exception
try
{
    await _repository.SaveAsync(entity);
}
catch (DbException ex)
{
    throw new SQLException.ItemAlreadyExists("Entity already exists", ex);
}

// ❌ Bad - Lose original exception
try
{
    await _repository.SaveAsync(entity);
}
catch (DbException)
{
    throw new SQLException.ItemAlreadyExists("Entity already exists");
}
```

### 4. Handle Exceptions at Appropriate Levels

```csharp
// ✅ Good - Handle at controller level
[HttpPost]
public async Task<ActionResult> CreateUser(CreateUserRequest request)
{
    try
    {
        var user = await _userService.CreateUserAsync(request);
        return Ok(user);
    }
    catch (SQLException.ItemAlreadyExists ex)
    {
        return Conflict(ex.Message);
    }
}

// ❌ Bad - Handle too early in service
public async Task<User> CreateUserAsync(CreateUserRequest request)
{
    try
    {
        // ... logic
    }
    catch (SQLException.ItemAlreadyExists)
    {
        return null; // Loses important error information
    }
}
```

### 5. Log Exceptions Appropriately

```csharp
try
{
    await ProcessRequest();
}
catch (HttpResponseException ex)
{
    // Client errors - log as warning
    _logger.LogWarning("Client error: {Message}", ex.Message);
    throw;
}
catch (LDAPException.ConnectionError ex)
{
    // Infrastructure errors - log as error
    _logger.LogError(ex, "LDAP connection failed");
    throw;
}
catch (Exception ex)
{
    // Unexpected errors - log as critical
    _logger.LogCritical(ex, "Unexpected error occurred");
    throw;
}
```

Custom exceptions provide a structured way to handle different types of errors in your application, making it easier to provide appropriate responses to clients and debug issues when they occur.
