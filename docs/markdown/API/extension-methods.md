# Extension Methods

Extension methods in the NextQuestionnaire Backend provide convenient ways to extend existing types with additional functionality. This guide covers the existing extension methods and how to create your own.

## What are Extension Methods?

Extension methods allow you to add new methods to existing types without modifying their original definition or creating derived types. They provide a clean way to enhance functionality and improve code readability.

## Existing Extension Methods

### ActiveQuestionnaireExtensions

The `ActiveQuestionnaireExtensions` class provides methods to convert `ActiveQuestionnaireBase` objects into role-specific DTOs.

#### ToActiveQuestionnaireStudentDTO

Converts an `ActiveQuestionnaireBase` to a student-focused DTO that excludes teacher-specific information.

```csharp
public static ActiveQuestionnaireStudentBase ToActiveQuestionnaireStudentDTO(this ActiveQuestionnaireBase activeQuestionnaire)
{
    return new()
    {
        Id = activeQuestionnaire.Id,
        Title = activeQuestionnaire.Title,
        Description = activeQuestionnaire.Description,
        ActivatedAt = activeQuestionnaire.ActivatedAt,
        Student = activeQuestionnaire.Student,
        StudentCompletedAt = activeQuestionnaire.StudentCompletedAt
    };
}
```

**Usage Example:**
```csharp
public async Task<List<ActiveQuestionnaireStudentBase>> GetStudentQuestionnaires(Guid studentId)
{
    var questionnaires = await _repository.GetActiveQuestionnairesForStudent(studentId);
    
    return questionnaires.Select(q => q.ToActiveQuestionnaireStudentDTO()).ToList();
}
```

#### ToActiveQuestionnaireTeacherDTO

Converts an `ActiveQuestionnaireBase` to a teacher-focused DTO that includes comprehensive information for both students and teachers.

```csharp
public static ActiveQuestionnaireTeacherBase ToActiveQuestionnaireTeacherDTO(this ActiveQuestionnaireBase activeQuestionnaire)
{
    return new()
    {
        Id = activeQuestionnaire.Id,
        Title = activeQuestionnaire.Title,
        Description = activeQuestionnaire.Description,
        ActivatedAt = activeQuestionnaire.ActivatedAt,
        Student = activeQuestionnaire.Student,
        StudentCompletedAt = activeQuestionnaire.StudentCompletedAt,
        Teacher = activeQuestionnaire.Teacher,
        TeacherCompletedAt = activeQuestionnaire.TeacherCompletedAt
    };
}
```

**Usage Example:**
```csharp
public async Task<List<ActiveQuestionnaireTeacherBase>> GetTeacherQuestionnaires(Guid teacherId)
{
    var questionnaires = await _repository.GetActiveQuestionnairesForTeacher(teacherId);
    
    return questionnaires.Select(q => q.ToActiveQuestionnaireTeacherDTO()).ToList();
}
```

### BasicUserInfoWithObjectGuidExtensions

The `BasicUserInfoWithObjectGuidExtensions` class provides conversion methods for LDAP user objects.

#### ToUserBaseDto

Converts a `BasicUserInfoWithObjectGuid` instance to a `LdapUserBase` DTO, mapping LDAP-specific fields to application DTOs.

```csharp
public static LdapUserBase ToUserBaseDto(this BasicUserInfoWithObjectGuid user)
{
    return new()
    {
        Id = new(user.ObjectGUID.ByteValue),
        FullName = user.Name.StringValue,
        UserName = user.Username.StringValue
    };
}
```

**Field Mappings:**
- `ObjectGUID.ByteValue` → `Id` (converted to Guid)
- `Name.StringValue` → `FullName`
- `Username.StringValue` → `UserName`

**Usage Example:**
```csharp
public async Task<List<LdapUserBase>> SearchUsers(string searchTerm)
{
    var ldapUsers = await _ldapService.SearchUsers(searchTerm);
    
    return ldapUsers.Select(user => user.ToUserBaseDto()).ToList();
}
```

## Creating Custom Extension Methods

### Basic Extension Method Structure

```csharp
namespace API.Extensions;

public static class StringExtensions
{
    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return emailRegex.IsMatch(email);
    }
}
```

### Example: Collection Extensions

```csharp
namespace API.Extensions;

/// <summary>
/// Provides extension methods for IEnumerable collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Checks if a collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection to check.</param>
    /// <returns>True if the collection is null or empty; otherwise, false.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        return source == null || !source.Any();
    }
    
    /// <summary>
    /// Converts a collection to a paginated result.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="pageNumber">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated result containing the requested page of data.</returns>
    public static PaginatedResult<T> ToPaginatedResult<T>(
        this IEnumerable<T> source, 
        int pageNumber, 
        int pageSize)
    {
        var totalCount = source.Count();
        var items = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return new PaginatedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
    
    /// <summary>
    /// Splits a collection into batches of the specified size.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The source collection.</param>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An enumerable of batches.</returns>
    public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source, 
        int batchSize)
    {
        if (batchSize <= 0)
            throw new ArgumentException("Batch size must be greater than zero.", nameof(batchSize));
            
        return source
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / batchSize)
            .Select(g => g.Select(x => x.item));
    }
}
```

**Usage Examples:**
```csharp
// Check if collection is null or empty
var users = await GetUsersAsync();
if (users.IsNullOrEmpty())
{
    return NotFound("No users found");
}

// Paginate results
var paginatedUsers = users.ToPaginatedResult(pageNumber: 1, pageSize: 10);

// Process in batches
var allUsers = await GetAllUsersAsync();
foreach (var batch in allUsers.Batch(100))
{
    await ProcessUserBatch(batch);
}
```

### Example: DateTime Extensions

```csharp
namespace API.Extensions;

/// <summary>
/// Provides extension methods for DateTime operations.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Converts a DateTime to Unix timestamp (seconds since epoch).
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <returns>The Unix timestamp.</returns>
    public static long ToUnixTimestamp(this DateTime dateTime)
    {
        return ((DateTimeOffset)dateTime).ToUnixTimeSeconds();
    }
    
    /// <summary>
    /// Checks if a DateTime is within a specified range.
    /// </summary>
    /// <param name="dateTime">The DateTime to check.</param>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    /// <returns>True if the DateTime is within the range; otherwise, false.</returns>
    public static bool IsBetween(this DateTime dateTime, DateTime start, DateTime end)
    {
        return dateTime >= start && dateTime <= end;
    }
    
    /// <summary>
    /// Gets the start of the day (00:00:00) for the given DateTime.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>A DateTime representing the start of the day.</returns>
    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;
    }
    
    /// <summary>
    /// Gets the end of the day (23:59:59.999) for the given DateTime.
    /// </summary>
    /// <param name="dateTime">The DateTime.</param>
    /// <returns>A DateTime representing the end of the day.</returns>
    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.Date.AddDays(1).AddTicks(-1);
    }
    
    /// <summary>
    /// Returns a human-readable relative time string (e.g., "2 hours ago").
    /// </summary>
    /// <param name="dateTime">The DateTime to convert.</param>
    /// <returns>A relative time string.</returns>
    public static string ToRelativeTime(this DateTime dateTime)
    {
        var timeSpan = DateTime.UtcNow - dateTime;
        
        return timeSpan.TotalDays switch
        {
            >= 365 => $"{(int)(timeSpan.TotalDays / 365)} year(s) ago",
            >= 30 => $"{(int)(timeSpan.TotalDays / 30)} month(s) ago",
            >= 1 => $"{(int)timeSpan.TotalDays} day(s) ago",
            _ => timeSpan.TotalHours switch
            {
                >= 1 => $"{(int)timeSpan.TotalHours} hour(s) ago",
                _ => timeSpan.TotalMinutes switch
                {
                    >= 1 => $"{(int)timeSpan.TotalMinutes} minute(s) ago",
                    _ => "Just now"
                }
            }
        };
    }
}
```

**Usage Examples:**
```csharp
var questionnaire = await GetQuestionnaireAsync(id);
var timestamp = questionnaire.CreatedAt.ToUnixTimestamp();
var relativeTime = questionnaire.CreatedAt.ToRelativeTime(); // "2 hours ago"

var isWithinRange = DateTime.UtcNow.IsBetween(
    questionnaire.ActivatedAt, 
    questionnaire.ExpiresAt);
```

### Example: Model Extensions

```csharp
namespace API.Extensions;

/// <summary>
/// Provides extension methods for converting between domain models and DTOs.
/// </summary>
public static class ModelExtensions
{
    /// <summary>
    /// Converts a QuestionnaireTemplate to a QuestionnaireTemplateResponse DTO.
    /// </summary>
    /// <param name="template">The template to convert.</param>
    /// <returns>A QuestionnaireTemplateResponse DTO.</returns>
    public static QuestionnaireTemplateResponse ToResponseDto(this QuestionnaireTemplate template)
    {
        return new QuestionnaireTemplateResponse
        {
            Id = template.Id,
            Title = template.Title,
            Description = template.Description,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            Questions = template.Questions?.Select(q => q.ToResponseDto()).ToList() ?? new List<QuestionResponse>()
        };
    }
    
    /// <summary>
    /// Converts a Question to a QuestionResponse DTO.
    /// </summary>
    /// <param name="question">The question to convert.</param>
    /// <returns>A QuestionResponse DTO.</returns>
    public static QuestionResponse ToResponseDto(this Question question)
    {
        return new QuestionResponse
        {
            Id = question.Id,
            Text = question.Text,
            Type = question.Type,
            Required = question.Required,
            Order = question.Order,
            Options = question.Options?.Select(o => o.ToResponseDto()).ToList() ?? new List<QuestionOptionResponse>()
        };
    }
    
    /// <summary>
    /// Converts a QuestionOption to a QuestionOptionResponse DTO.
    /// </summary>
    /// <param name="option">The option to convert.</param>
    /// <returns>A QuestionOptionResponse DTO.</returns>
    public static QuestionOptionResponse ToResponseDto(this QuestionOption option)
    {
        return new QuestionOptionResponse
        {
            Id = option.Id,
            Text = option.Text,
            Value = option.Value,
            Order = option.Order
        };
    }
    
    /// <summary>
    /// Checks if a questionnaire template is valid for activation.
    /// </summary>
    /// <param name="template">The template to validate.</param>
    /// <returns>True if the template is valid; otherwise, false.</returns>
    public static bool IsValidForActivation(this QuestionnaireTemplate template)
    {
        return !string.IsNullOrWhiteSpace(template.Title) &&
               template.Questions != null &&
               template.Questions.Any() &&
               template.Questions.All(q => q.IsValid());
    }
    
    /// <summary>
    /// Checks if a question is valid.
    /// </summary>
    /// <param name="question">The question to validate.</param>
    /// <returns>True if the question is valid; otherwise, false.</returns>
    public static bool IsValid(this Question question)
    {
        if (string.IsNullOrWhiteSpace(question.Text))
            return false;
            
        // Multiple choice questions must have options
        if (question.Type == QuestionType.MultipleChoice || question.Type == QuestionType.Checkbox)
        {
            return question.Options != null && question.Options.Any();
        }
        
        return true;
    }
}
```

**Usage Examples:**
```csharp
// Convert models to DTOs
var template = await _repository.GetTemplateByIdAsync(id);
var responseDto = template.ToResponseDto();

// Validate before activation
if (template.IsValidForActivation())
{
    await _service.ActivateTemplateAsync(template.Id);
}
else
{
    throw new ValidationException("Template is not valid for activation");
}
```

### Example: HttpContext Extensions

```csharp
namespace API.Extensions;

/// <summary>
/// Provides extension methods for HttpContext operations.
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// Gets the user ID from the current JWT token claims.
    /// </summary>
    /// <param name="context">The HttpContext.</param>
    /// <returns>The user ID if found; otherwise, null.</returns>
    public static Guid? GetUserId(this HttpContext context)
    {
        var userIdClaim = context.User.FindFirst("sub") ?? context.User.FindFirst("user_id");
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }
    
    /// <summary>
    /// Gets the user role from the current JWT token claims.
    /// </summary>
    /// <param name="context">The HttpContext.</param>
    /// <returns>The user role if found; otherwise, null.</returns>
    public static string? GetUserRole(this HttpContext context)
    {
        return context.User.FindFirst("role")?.Value;
    }
    
    /// <summary>
    /// Checks if the current user has the specified role.
    /// </summary>
    /// <param name="context">The HttpContext.</param>
    /// <param name="role">The role to check.</param>
    /// <returns>True if the user has the role; otherwise, false.</returns>
    public static bool HasRole(this HttpContext context, string role)
    {
        return context.GetUserRole()?.Equals(role, StringComparison.OrdinalIgnoreCase) == true;
    }
    
    /// <summary>
    /// Gets the client IP address from the request.
    /// </summary>
    /// <param name="context">The HttpContext.</param>
    /// <returns>The client IP address.</returns>
    public static string? GetClientIpAddress(this HttpContext context)
    {
        // Check for forwarded IP first (behind proxy)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }
        
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }
        
        return context.Connection.RemoteIpAddress?.ToString();
    }
}
```

**Usage in Controllers:**
```csharp
[HttpGet]
public async Task<ActionResult<List<QuestionnaireResponse>>> GetUserQuestionnaires()
{
    var userId = HttpContext.GetUserId();
    if (userId == null)
    {
        return Unauthorized();
    }
    
    var role = HttpContext.GetUserRole();
    var questionnaires = role?.ToLower() switch
    {
        "student" => await _service.GetStudentQuestionnaires(userId.Value),
        "teacher" => await _service.GetTeacherQuestionnaires(userId.Value),
        _ => throw new UnauthorizedAccessException("Invalid role")
    };
    
    return Ok(questionnaires);
}
```

## Best Practices

### 1. Use Appropriate Namespaces
```csharp
// ✅ Good - Specific namespace
namespace API.Extensions;

// ❌ Bad - Too generic
namespace Extensions;
```

### 2. Make Extension Methods Static
```csharp
// ✅ Good - Static class and method
public static class StringExtensions
{
    public static bool IsValidEmail(this string email) { }
}

// ❌ Bad - Non-static
public class StringExtensions
{
    public bool IsValidEmail(this string email) { } // Won't work
}
```

### 3. Use Descriptive Names
```csharp
// ✅ Good - Clear purpose
public static QuestionnaireResponse ToResponseDto(this Questionnaire questionnaire)
public static bool IsValidForActivation(this QuestionnaireTemplate template)

// ❌ Bad - Unclear purpose
public static QuestionnaireResponse Convert(this Questionnaire questionnaire)
public static bool Check(this QuestionnaireTemplate template)
```

### 4. Handle Null Cases
```csharp
// ✅ Good - Null-safe
public static string ToSafeString(this object? obj)
{
    return obj?.ToString() ?? string.Empty;
}

public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
{
    return source == null || !source.Any();
}

// ❌ Bad - Might throw NullReferenceException
public static string ToSafeString(this object obj)
{
    return obj.ToString(); // Throws if obj is null
}
```

### 5. Provide Comprehensive Documentation
```csharp
/// <summary>
/// Converts an ActiveQuestionnaireBase object to an ActiveQuestionnaireStudentBase DTO.
/// </summary>
/// <param name="activeQuestionnaire">The active questionnaire to convert.</param>
/// <returns>
/// An ActiveQuestionnaireStudentBase DTO containing student-relevant information including
/// questionnaire details, activation time, student information, and student completion status.
/// </returns>
/// <remarks>
/// This method creates a student-focused view of the questionnaire data, excluding teacher-specific
/// information such as teacher details and teacher completion status.
/// </remarks>
public static ActiveQuestionnaireStudentBase ToActiveQuestionnaireStudentDTO(this ActiveQuestionnaireBase activeQuestionnaire)
```

Extension methods provide a powerful way to enhance existing types with additional functionality while keeping your code clean and readable. They're particularly useful for conversion operations, validation logic, and utility functions.
