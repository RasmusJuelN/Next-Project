# Database Extensions Guide

Extensions in the Database project provide utility methods and mappings that extend the functionality of existing classes. They primarily handle data transformation between models and DTOs, query operations, and object mapping.

## Overview

The Extensions directory contains several categories of extension methods:

- **Mapper Extensions**: Convert between database models and DTOs
- **Query Extensions**: Enhance LINQ querying capabilities
- **Utility Extensions**: Provide common operations and transformations

## Types of Extensions

### Model to DTO Mappers

These extensions handle the conversion from Entity Framework models to Data Transfer Objects (DTOs):

- **`UserBaseModelMapper`**: Maps user models to user DTOs
- **`UserMapper`** / **`UserAddMapper`**: Handle full user mapping operations
- **`ActiveQuestionnaireMapper`**: Maps questionnaire models to DTOs
- **`QuestionnaireTemplateMapper`** / **`QuestionnaireTemplateModelMapper`**: Template conversions
- **`ApplicationLogsModelMapper`**: Log entry mappings

### Query Extensions

- **`IQueryableExtensions`**: Enhances LINQ querying with custom operations
- **`GetQueryMethod`**: Provides dynamic query method resolution

## Creating Mapper Extensions

### Basic Mapper Pattern

```csharp
using Database.DTO.YourEntity;
using Database.Models;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping YourEntity database models to DTOs.
/// </summary>
public static class YourEntityModelMapper
{
    /// <summary>
    /// Converts a YourEntityModel to a basic DTO.
    /// </summary>
    /// <param name="entity">The entity model from the database.</param>
    /// <returns>A DTO containing essential entity information.</returns>
    public static YourEntityDto ToDto(this YourEntityModel entity)
    {
        return new YourEntityDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            Status = entity.Status
        };
    }

    /// <summary>
    /// Converts a YourEntityModel to a detailed DTO with related data.
    /// </summary>
    /// <param name="entity">The entity model with included related data.</param>
    /// <returns>A comprehensive DTO with all related information.</returns>
    public static YourEntityDetailDto ToDetailDto(this YourEntityModel entity)
    {
        return new YourEntityDetailDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            Status = entity.Status,
            RelatedItems = entity.RelatedEntities?.Select(r => r.ToDto()).ToList() ?? []
        };
    }

    /// <summary>
    /// Converts a list of entities to DTOs efficiently.
    /// </summary>
    /// <param name="entities">The list of entity models.</param>
    /// <returns>A list of DTOs.</returns>
    public static List<YourEntityDto> ToDtoList(this IEnumerable<YourEntityModel> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}
```

### Advanced Mapping with Conditional Logic

```csharp
public static YourEntityDto ToDto(this YourEntityModel entity, bool includeDetails = false)
{
    var dto = new YourEntityDto
    {
        Id = entity.Id,
        Name = entity.Name,
        Status = entity.Status
    };

    if (includeDetails)
    {
        dto.Description = entity.Description;
        dto.AdditionalInfo = entity.CalculateAdditionalInfo();
    }

    return dto;
}
```

## Creating Query Extensions

### Basic Query Extension

```csharp
using System.Linq.Expressions;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for enhanced LINQ querying.
/// </summary>
public static class YourEntityQueryExtensions
{
    /// <summary>
    /// Filters entities by active status.
    /// </summary>
    public static IQueryable<TEntity> WhereActive<TEntity>(this IQueryable<TEntity> query) 
        where TEntity : IHasStatus
    {
        return query.Where(e => e.Status == EntityStatus.Active);
    }

    /// <summary>
    /// Filters entities created within a date range.
    /// </summary>
    public static IQueryable<TEntity> WhereCreatedBetween<TEntity>(
        this IQueryable<TEntity> query, 
        DateTime startDate, 
        DateTime endDate) 
        where TEntity : IHasCreatedDate
    {
        return query.Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate);
    }

    /// <summary>
    /// Applies standard ordering for pagination.
    /// </summary>
    public static IQueryable<TEntity> OrderByStandard<TEntity>(this IQueryable<TEntity> query) 
        where TEntity : IHasCreatedDate
    {
        return query.OrderByDescending(e => e.CreatedAt).ThenBy(e => e.Id);
    }
}
```

## Best Practices

### Mapper Extensions

1. **Keep mappings simple**: Focus on property assignment without complex logic
2. **Handle null values**: Always check for null references
3. **Use appropriate DTO types**: Create different DTOs for different contexts (list, detail, create, update)
4. **Include documentation**: Explain when to use each mapping method

### Query Extensions

1. **Generic where possible**: Use generic constraints to apply to multiple entity types
2. **Composable methods**: Ensure extensions can be chained together
3. **Performance considerations**: Avoid materializing queries unless necessary
4. **Consistent naming**: Use clear, descriptive method names

### Example: Complete Extension File

```csharp
using Database.DTO.Course;
using Database.Models;

namespace Database.Extensions;

/// <summary>
/// Provides extension methods for mapping Course database models to DTOs.
/// </summary>
public static class CourseModelMapper
{
    /// <summary>
    /// Converts a CourseModel to a basic CourseDto.
    /// </summary>
    public static CourseDto ToDto(this CourseModel course)
    {
        return new CourseDto
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            Description = course.Description,
            CreatedAt = course.CreatedAt
        };
    }

    /// <summary>
    /// Converts a CourseModel to a detailed DTO with student and teacher counts.
    /// </summary>
    public static CourseDetailDto ToDetailDto(this CourseModel course)
    {
        return new CourseDetailDto
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name,
            Description = course.Description,
            CreatedAt = course.CreatedAt,
            StudentCount = course.Students?.Count ?? 0,
            TeacherCount = course.Teachers?.Count ?? 0,
            Students = course.Students?.Select(s => s.ToBaseDto()).ToList() ?? [],
            Teachers = course.Teachers?.Select(t => t.ToBaseDto()).ToList() ?? []
        };
    }
}
```

Extensions provide a clean, maintainable way to handle data transformation and query operations while keeping the core models focused on their primary responsibilities.
