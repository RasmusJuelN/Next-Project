# Database Enums Guide

Enums in the Database project define fixed sets of values that represent system states, user roles, permissions, and configuration options. They provide type safety and ensure data consistency across the application.

## Overview

Enums are used throughout the system to define:

- **User Roles and Permissions**: Define access levels and capabilities
- **Entity States**: Track lifecycle status of various entities
- **Configuration Options**: Provide predefined choices for system behavior
- **Ordering and Filtering**: Define sort orders and filter criteria

## Existing Enums

### UserRoles

Defines the different roles users can have in the system:

```csharp
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRoles
{
    Student,
    Teacher,
    Admin
}
```

### UserPermissions

Defines granular permissions for fine-grained access control:

```csharp
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserPermissions
{
    None = 0,
    CanModifyQuestionnaireTemplates = 1,
    CanModifyApplicationSettings = 2,
    CanViewApplicationLogs = 4,
    CanAssignQuestionnaires = 8,
    CanRespondToQuestionnaires = 16,
    CanModifyPermissions = 32,

    // Presets
    Student = CanRespondToQuestionnaires,
    Teacher = CanRespondToQuestionnaires | CanAssignQuestionnaires,
    Admin = CanAssignQuestionnaires | CanModifyQuestionnaireTemplates | CanModifyApplicationSettings | CanViewApplicationLogs,
    SuperAdmin = Admin | CanModifyPermissions
}
```

### Ordering Options

Define sorting criteria for entity queries:

- **`ActiveQuestionnaireOrderingOptions`**: Sort options for active questionnaires
- **`TemplateOrderingOptions`**: Sort options for questionnaire templates

## Creating Enums

### Basic Enum Structure

```csharp
using System.Text.Json.Serialization;

namespace Database.Enums;

/// <summary>
/// Represents the different statuses an entity can have.
/// </summary>
/// <remarks>
/// This enum is serialized as string values in JSON format for better API readability.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntityStatus
{
    /// <summary>
    /// Entity is active and available for use.
    /// </summary>
    Active,

    /// <summary>
    /// Entity is temporarily inactive but can be reactivated.
    /// </summary>
    Inactive,

    /// <summary>
    /// Entity is pending approval or processing.
    /// </summary>
    Pending,

    /// <summary>
    /// Entity has been archived and is read-only.
    /// </summary>
    Archived,

    /// <summary>
    /// Entity has been deleted (soft delete).
    /// </summary>
    Deleted
}
```

### Flags Enum for Permissions

```csharp
/// <summary>
/// Defines granular permissions that can be combined using bitwise operations.
/// </summary>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CoursePermissions
{
    None = 0,
    
    /// <summary>
    /// Can view course information.
    /// </summary>
    View = 1,
    
    /// <summary>
    /// Can create new courses.
    /// </summary>
    Create = 2,
    
    /// <summary>
    /// Can edit existing courses.
    /// </summary>
    Edit = 4,
    
    /// <summary>
    /// Can delete courses.
    /// </summary>
    Delete = 8,
    
    /// <summary>
    /// Can manage course enrollments.
    /// </summary>
    ManageEnrollments = 16,
    
    /// <summary>
    /// Can view course analytics and reports.
    /// </summary>
    ViewAnalytics = 32,
    
    /// <summary>
    /// All course permissions combined.
    /// </summary>
    All = View | Create | Edit | Delete | ManageEnrollments | ViewAnalytics
}
```

### Ordering Enums

```csharp
/// <summary>
/// Defines the available ordering options for course queries.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CourseOrderingOptions
{
    /// <summary>
    /// Order by course name alphabetically.
    /// </summary>
    NameAscending,
    
    /// <summary>
    /// Order by course name reverse alphabetically.
    /// </summary>
    NameDescending,
    
    /// <summary>
    /// Order by creation date, newest first.
    /// </summary>
    CreatedDateDescending,
    
    /// <summary>
    /// Order by creation date, oldest first.
    /// </summary>
    CreatedDateAscending,
    
    /// <summary>
    /// Order by enrollment count, highest first.
    /// </summary>
    EnrollmentCountDescending,
    
    /// <summary>
    /// Order by course code alphabetically.
    /// </summary>
    CodeAscending
}
```

### State Machine Enums

```csharp
/// <summary>
/// Represents the lifecycle states of a questionnaire.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum QuestionnaireState
{
    /// <summary>
    /// Questionnaire is being created or edited.
    /// </summary>
    Draft,
    
    /// <summary>
    /// Questionnaire is ready for review.
    /// </summary>
    PendingReview,
    
    /// <summary>
    /// Questionnaire has been approved and can be activated.
    /// </summary>
    Approved,
    
    /// <summary>
    /// Questionnaire is currently active and accepting responses.
    /// </summary>
    Active,
    
    /// <summary>
    /// Questionnaire has been completed by all participants.
    /// </summary>
    Completed,
    
    /// <summary>
    /// Questionnaire has been cancelled or suspended.
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// Questionnaire has been archived for historical purposes.
    /// </summary>
    Archived
}
```

## Enum Usage Patterns

### In Models

```csharp
public class CourseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [Required]
    public EntityStatus Status { get; set; } = EntityStatus.Active;
    
    public CourseType Type { get; set; }
}
```

### In DTOs

```csharp
public record class CourseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public EntityStatus Status { get; set; }
    public CourseType Type { get; set; }
}
```

### In Query Operations

```csharp
// Using enum in LINQ queries
var activeCourses = await _context.Courses
    .Where(c => c.Status == EntityStatus.Active)
    .ToListAsync();

// Using flags enum for permission checking
public bool HasPermission(CoursePermissions userPermissions, CoursePermissions requiredPermission)
{
    return userPermissions.HasFlag(requiredPermission);
}

// Using ordering enum
public IQueryable<Course> ApplyOrdering(IQueryable<Course> query, CourseOrderingOptions ordering)
{
    return ordering switch
    {
        CourseOrderingOptions.NameAscending => query.OrderBy(c => c.Name),
        CourseOrderingOptions.NameDescending => query.OrderByDescending(c => c.Name),
        CourseOrderingOptions.CreatedDateDescending => query.OrderByDescending(c => c.CreatedAt),
        CourseOrderingOptions.CreatedDateAscending => query.OrderBy(c => c.CreatedAt),
        CourseOrderingOptions.EnrollmentCountDescending => query.OrderByDescending(c => c.Students.Count),
        CourseOrderingOptions.CodeAscending => query.OrderBy(c => c.Code),
        _ => query.OrderBy(c => c.Name)
    };
}
```

### Extension Methods for Enums

```csharp
namespace Database.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets a human-readable description of the enum value.
    /// </summary>
    public static string GetDescription(this EntityStatus status)
    {
        return status switch
        {
            EntityStatus.Active => "Active and available",
            EntityStatus.Inactive => "Temporarily disabled",
            EntityStatus.Pending => "Awaiting approval",
            EntityStatus.Archived => "Archived for reference",
            EntityStatus.Deleted => "Marked for deletion",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Checks if a status allows editing operations.
    /// </summary>
    public static bool IsEditable(this EntityStatus status)
    {
        return status is EntityStatus.Active or EntityStatus.Pending;
    }

    /// <summary>
    /// Gets all individual permissions from a flags enum.
    /// </summary>
    public static IEnumerable<CoursePermissions> GetIndividualPermissions(this CoursePermissions permissions)
    {
        return Enum.GetValues<CoursePermissions>()
            .Where(p => p != CoursePermissions.None && p != CoursePermissions.All)
            .Where(permissions.HasFlag);
    }
}
```

## Entity Framework Configuration

### Enum to String Conversion

```csharp
// In DbContext.OnModelCreating
modelBuilder.Entity<CourseModel>(entity =>
{
    // Store enum as string in database
    entity.Property(e => e.Status)
        .HasConversion<string>();
    
    entity.Property(e => e.Type)
        .HasConversion<string>();
});
```

### Default Values

```csharp
modelBuilder.Entity<CourseModel>(entity =>
{
    entity.Property(e => e.Status)
        .HasDefaultValue(EntityStatus.Active);
});
```

## Best Practices

### Enum Design

1. **Use meaningful names**: Choose descriptive enum and value names
2. **Include documentation**: Add XML comments for each enum value
3. **Consider extensibility**: Design enums that can grow without breaking changes
4. **Use flags appropriately**: Only use `[Flags]` for enums representing combinable options

### JSON Serialization

1. **String conversion**: Use `JsonStringEnumConverter` for API-friendly serialization
2. **Consistent naming**: Ensure enum names work well in JSON format
3. **Handle unknown values**: Consider how to handle undefined enum values from external sources

### Database Storage

1. **String storage**: Store enums as strings for better readability and maintainability
2. **Constraints**: Add database constraints to ensure valid enum values
3. **Migrations**: Handle enum changes carefully in database migrations

### Example: Complete Enum Implementation

```csharp
using System.Text.Json.Serialization;

namespace Database.Enums;

/// <summary>
/// Represents the priority levels for tasks and assignments.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Priority
{
    /// <summary>
    /// Low priority - can be addressed when time permits.
    /// </summary>
    Low = 1,
    
    /// <summary>
    /// Normal priority - standard processing order.
    /// </summary>
    Normal = 2,
    
    /// <summary>
    /// High priority - should be addressed soon.
    /// </summary>
    High = 3,
    
    /// <summary>
    /// Critical priority - requires immediate attention.
    /// </summary>
    Critical = 4,
    
    /// <summary>
    /// Emergency priority - drop everything else.
    /// </summary>
    Emergency = 5
}

// Usage in extension methods
public static class PriorityExtensions
{
    public static string GetDisplayName(this Priority priority) => priority switch
    {
        Priority.Low => "Low Priority",
        Priority.Normal => "Normal Priority", 
        Priority.High => "High Priority",
        Priority.Critical => "Critical Priority",
        Priority.Emergency => "Emergency Priority",
        _ => priority.ToString()
    };

    public static bool RequiresImmediateAction(this Priority priority) =>
        priority is Priority.Critical or Priority.Emergency;
}
```

Enums provide type safety, improve code readability, and ensure data consistency across your application while remaining flexible for future extensions.
