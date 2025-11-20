# Database DTOs Guide

Data Transfer Objects (DTOs) in the Database project define the structure for data exchange between layers. They provide a clean separation between internal database models and external API contracts, ensuring data integrity and security.

## Overview

DTOs are organized by domain area and serve different purposes:

- **Request DTOs**: Define input data structure for API operations
- **Response DTOs**: Define output data structure for API responses  
- **Internal DTOs**: Used for inter-layer communication within the application

## Existing DTO Categories

### User DTOs (`Database.DTO.User`)

- **`UserBase`**: Basic user information without sensitive data
- **`FullUser`**: Complete user details including roles and permissions
- **`Add`**: User creation request structure

### ActiveQuestionnaire DTOs (`Database.DTO.ActiveQuestionnaire`)

- **`ActiveQuestionnaireBase`**: Essential questionnaire information
- **`ActiveQuestionnaire`**: Complete questionnaire with questions and options
- **`Add`**: Questionnaire activation request
- **`Answer`** / **`AnswerSubmission`**: Response handling structures
- **`UserSpecificActiveQuestionnaire`**: Role-specific questionnaire views

### QuestionnaireTemplate DTOs (`Database.DTO.QuestionnaireTemplate`)

- **`TemplateBase`**: Basic template information
- **`Template`**: Complete template with questions
- **`Add`** / **`Update`** / **`Patch`**: Template modification structures

### ApplicationLog DTOs (`Database.DTO.ApplicationLog`)

- **`ApplicationLog`**: Log entry structure for monitoring and debugging

## Creating DTOs

### Basic DTO Structure

```csharp
namespace Database.DTO.YourDomain;

/// <summary>
/// Represents basic information about your entity.
/// Used for list displays and summary views.
/// </summary>
public record class YourEntityBase
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTime CreatedAt { get; set; }
}

/// <summary>
/// Represents complete information about your entity.
/// Extends the base DTO with additional details.
/// </summary>
public record class YourEntity : YourEntityBase
{
    public string? Description { get; set; }
    public required EntityStatus Status { get; set; }
    public DateTime? LastUpdated { get; set; }
    public List<RelatedItemDto> RelatedItems { get; set; } = [];
}
```

### Request DTOs

```csharp
/// <summary>
/// Request structure for creating a new entity.
/// Contains only the data required for creation.
/// </summary>
public record class AddYourEntity
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public required EntityStatus Status { get; set; }

    public List<Guid> RelatedItemIds { get; set; } = [];
}

/// <summary>
/// Request structure for updating an existing entity.
/// Contains all updatable fields.
/// </summary>
public record class UpdateYourEntity
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public required EntityStatus Status { get; set; }
}

/// <summary>
/// Request structure for partial updates (PATCH operations).
/// All fields are optional.
/// </summary>
public record class PatchYourEntity
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public EntityStatus? Status { get; set; }
}
```

### Response DTOs with Pagination

```csharp
/// <summary>
/// Paginated response structure for entity lists.
/// </summary>
public record class YourEntityPagedResponse
{
    public required List<YourEntityBase> Items { get; set; }
    public required int TotalCount { get; set; }
    public required int PageNumber { get; set; }
    public required int PageSize { get; set; }
    public required bool HasNextPage { get; set; }
    public required bool HasPreviousPage { get; set; }
}

/// <summary>
/// Keyset pagination response for efficient large dataset handling.
/// </summary>
public record class YourEntityKeysetResponse
{
    public required List<YourEntity> Items { get; set; }
    public required int Count { get; set; }
    public Guid? NextPageKey { get; set; }
    public Guid? PreviousPageKey { get; set; }
}
```

### Nested and Complex DTOs

```csharp
/// <summary>
/// Complex DTO that includes related entity information.
/// </summary>
public record class YourEntityWithDetails : YourEntity
{
    public UserBase? CreatedBy { get; set; }
    public UserBase? LastModifiedBy { get; set; }
    public List<CommentDto> Comments { get; set; } = [];
    public List<AttachmentDto> Attachments { get; set; } = [];
    public AuditTrailDto? AuditTrail { get; set; }
}

/// <summary>
/// Aggregated data DTO for reporting and analytics.
/// </summary>
public record class YourEntitySummary
{
    public required int TotalCount { get; set; }
    public required int ActiveCount { get; set; }
    public required int InactiveCount { get; set; }
    public required DateTime LastCreated { get; set; }
    public required Dictionary<string, int> StatusBreakdown { get; set; }
}
```

## DTO Design Patterns

### Inheritance Hierarchy

```csharp
// Base DTO with common properties
public record class EntityBase
{
    public required Guid Id { get; set; }
    public required DateTime CreatedAt { get; set; }
}

// Specific entity DTOs inherit from base
public record class UserDto : EntityBase
{
    public required string UserName { get; set; }
    public required string FullName { get; set; }
}

public record class QuestionnaireDto : EntityBase
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}
```

### Composition Pattern

```csharp
// Shared components
public record class AuditInfo
{
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public required Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public record class MetadataInfo
{
    public Dictionary<string, string> Tags { get; set; } = [];
    public string? Notes { get; set; }
}

// DTOs using composition
public record class DocumentDto
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required AuditInfo Audit { get; set; }
    public MetadataInfo? Metadata { get; set; }
}
```

### Role-Specific DTOs

```csharp
/// <summary>
/// User information visible to all authenticated users.
/// </summary>
public record class PublicUserDto
{
    public required string UserName { get; set; }
    public required string FullName { get; set; }
}

/// <summary>
/// User information visible only to the user themselves.
/// </summary>
public record class PrivateUserDto : PublicUserDto
{
    public required string Email { get; set; }
    public required UserPermissions Permissions { get; set; }
    public DateTime LastLoginAt { get; set; }
}

/// <summary>
/// User information visible to administrators.
/// </summary>
public record class AdminUserDto : PrivateUserDto
{
    public required Guid Guid { get; set; }
    public required UserRoles PrimaryRole { get; set; }
    public bool IsLocked { get; set; }
    public List<string> LoginHistory { get; set; } = [];
}
```

## Validation and Attributes

```csharp
using System.ComponentModel.DataAnnotations;

public record class CreateProductDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public required string Name { get; set; }

    [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
    public required decimal Price { get; set; }

    [EmailAddress(ErrorMessage = "Please provide a valid email address")]
    public string? ContactEmail { get; set; }

    [Url(ErrorMessage = "Please provide a valid URL")]
    public string? WebsiteUrl { get; set; }

    [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "SKU must be in format: ABC-1234")]
    public string? SKU { get; set; }
}
```

## Best Practices

### DTO Design

1. **Use records**: Prefer `record class` for immutability and value semantics
2. **Required properties**: Use `required` keyword for mandatory fields
3. **Meaningful names**: Use clear, descriptive names that indicate purpose
4. **Separate concerns**: Create different DTOs for different use cases (list vs detail)

### Property Guidelines

1. **Nullable appropriately**: Only make properties nullable when they can legitimately be null
2. **Collections**: Initialize collections to empty lists to prevent null reference issues
3. **Validation attributes**: Add appropriate validation for API DTOs
4. **Documentation**: Include XML documentation for all public DTOs

### Performance Considerations

1. **Minimal DTOs**: Only include necessary properties to reduce serialization overhead
2. **Avoid circular references**: Be careful with nested DTOs to prevent serialization issues
3. **Consider flattening**: Sometimes flat DTOs perform better than deeply nested ones

### Example: Complete DTO Set

```csharp
namespace Database.DTO.Course;

public record class CourseBase
{
    public required int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}

public record class Course : CourseBase
{
    public string? Description { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int StudentCount { get; set; }
    public int TeacherCount { get; set; }
}

public record class CourseDetail : Course
{
    public List<UserBase> Students { get; set; } = [];
    public List<UserBase> Teachers { get; set; } = [];
    public List<AssignmentBase> Assignments { get; set; } = [];
}

public record class AddCourse
{
    [Required, StringLength(10)]
    public required string Code { get; set; }
    
    [Required, StringLength(200)]
    public required string Name { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }
}
```

DTOs provide a clean contract for data exchange while protecting internal model structures and enabling API evolution without breaking changes.
