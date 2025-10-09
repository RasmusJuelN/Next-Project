# Database Attributes Guide

Attributes in the Database project provide metadata and configuration for query operations and data processing. They enable declarative programming patterns and custom behavior for entity properties and methods.

## Overview

The Attributes directory contains custom attributes that extend the functionality of the Entity Framework models and query operations:

- **`QueryMethodAttribute`**: Defines custom query behavior for fields in LINQ operations

## Existing Attributes

### QueryMethodAttribute

The `QueryMethodAttribute` is used to specify which query method should be applied to a field during LINQ operations:

```csharp
[AttributeUsage(AttributeTargets.Field)]
public class QueryMethodAttribute(string queryMethod) : Attribute
{
    public string QueryMethod = queryMethod;
}
```

**Usage Example:**
```csharp
public class SearchCriteria
{
    [QueryMethod("Contains")]
    private string searchTerm;

    [QueryMethod("Equals")]
    private int categoryId;

    [QueryMethod("GreaterThanOrEqual")]
    private DateTime fromDate;
}
```

## Creating Custom Attributes

### Validation Attributes

```csharp
using System.ComponentModel.DataAnnotations;

namespace Database.Attributes;

/// <summary>
/// Validates that a GUID property is not empty.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is Guid guid)
        {
            return guid != Guid.Empty;
        }
        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"The {name} field cannot be an empty GUID.";
    }
}
```

### Metadata Attributes

```csharp
namespace Database.Attributes;

/// <summary>
/// Specifies the default ordering for entity queries.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DefaultOrderingAttribute(string propertyName, bool descending = false) : Attribute
{
    public string PropertyName { get; } = propertyName;
    public bool Descending { get; } = descending;
}

/// <summary>
/// Marks a property as auditable for change tracking.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class AuditableAttribute : Attribute
{
    public bool TrackChanges { get; set; } = true;
    public string? DisplayName { get; set; }
}
```

### Query Builder Attributes

```csharp
namespace Database.Attributes;

/// <summary>
/// Specifies include paths for eager loading in repository queries.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class IncludePathAttribute(params string[] paths) : Attribute
{
    public string[] Paths { get; } = paths;
}

/// <summary>
/// Defines filter criteria for repository methods.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FilterCriteriaAttribute(string filterExpression) : Attribute
{
    public string FilterExpression { get; } = filterExpression;
}
```

## Usage Examples

### Model with Attributes

```csharp
using Database.Attributes;

namespace Database.Models;

[DefaultOrdering(nameof(CreatedAt), descending: true)]
public class ProductModel
{
    public int Id { get; set; }

    [Auditable(DisplayName = "Product Name")]
    public required string Name { get; set; }

    [Auditable]
    public decimal Price { get; set; }

    [NotEmptyGuid]
    public Guid CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }
}
```

### Repository with Attributes

```csharp
public class ProductRepository
{
    [IncludePath("Category", "Reviews")]
    public async Task<ProductModel?> GetProductWithDetailsAsync(int id)
    {
        // Implementation would use the IncludePath attribute
        // to automatically include related entities
    }

    [FilterCriteria("p => p.IsActive && p.Price > 0")]
    public async Task<List<ProductModel>> GetActiveProductsAsync()
    {
        // Implementation would apply the filter criteria
    }
}
```

## Processing Attributes at Runtime

### Reflection-Based Attribute Processing

```csharp
using System.Reflection;

namespace Database.Utils;

public static class AttributeProcessor
{
    public static IQueryable<T> ApplyDefaultOrdering<T>(IQueryable<T> query)
    {
        var entityType = typeof(T);
        var orderingAttribute = entityType.GetCustomAttribute<DefaultOrderingAttribute>();
        
        if (orderingAttribute != null)
        {
            var property = entityType.GetProperty(orderingAttribute.PropertyName);
            if (property != null)
            {
                // Apply ordering based on attribute configuration
                if (orderingAttribute.Descending)
                {
                    return query.OrderByDescending(e => EF.Property<object>(e, property.Name));
                }
                else
                {
                    return query.OrderBy(e => EF.Property<object>(e, property.Name));
                }
            }
        }
        
        return query;
    }

    public static string[] GetIncludePaths(MethodInfo method)
    {
        var includeAttribute = method.GetCustomAttribute<IncludePathAttribute>();
        return includeAttribute?.Paths ?? [];
    }
}
```

## Best Practices

### Attribute Design

1. **Single Responsibility**: Each attribute should have one clear purpose
2. **Meaningful Names**: Use descriptive names that clearly indicate purpose
3. **Documentation**: Include comprehensive XML documentation
4. **Validation**: Validate attribute parameters where appropriate

### Usage Guidelines

1. **Apply Sparingly**: Don't over-use attributes; prefer explicit code when complexity is high
2. **Runtime Performance**: Consider the performance impact of reflection-based attribute processing
3. **Testing**: Ensure attributes work correctly with your query execution pipeline
4. **Consistency**: Use consistent naming patterns across related attributes

### Example: Complete Attribute Implementation

```csharp
namespace Database.Attributes;

/// <summary>
/// Specifies soft delete behavior for entity types.
/// When applied to a model, queries will automatically filter out deleted records.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SoftDeleteAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the property that indicates deletion status.
    /// Default is "IsDeleted".
    /// </summary>
    public string PropertyName { get; set; } = "IsDeleted";

    /// <summary>
    /// Gets or sets whether soft delete filtering is enabled by default.
    /// Default is true.
    /// </summary>
    public bool EnabledByDefault { get; set; } = true;
}

// Usage:
[SoftDelete(PropertyName = "DeletedAt", EnabledByDefault = true)]
public class DocumentModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime? DeletedAt { get; set; }
}
```

Attributes provide a powerful way to add metadata and behavior to your database models and operations while maintaining clean, readable code.
