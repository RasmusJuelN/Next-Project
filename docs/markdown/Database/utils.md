# Database Utils Guide

Utilities in the Database project provide helper classes and methods that support common database operations, design-time services, and data seeding functionality. They encapsulate reusable logic that doesn't belong in models or repositories.

## Overview

The Utils directory can contain utility classes that provide:

- **Design-Time Services**: Support Entity Framework migrations and tooling
- **Data Seeding**: Automated database initialization with default data
- **Helper Methods**: Common operations and transformations
- **Factory Classes**: Object creation and configuration utilities

## Existing Utils

### SeederHelper

Automatically discovers and executes data seeders that implement the `IDataSeeder` interface:

```csharp
public class SeederHelper(ModelBuilder modelBuilder)
{
    public void Seed()
    {
        // Uses reflection to find all IDataSeeder implementations
        // Instantiates and executes each seeder
    }
}
```

### DesignTimeFactory

Provides Entity Framework design-time services for migrations and tooling:

```csharp
public class DesignTimeFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        // Configures context for design-time operations
    }
}
```

## Creating Utility Classes

### Database Helper Utilities

```csharp
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Database.Utils;

/// <summary>
/// Provides utility methods for common database operations.
/// </summary>
public static class DatabaseHelper
{
    /// <summary>
    /// Executes a query with retry logic for transient failures.
    /// </summary>
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation, 
        int maxRetries = 3, 
        TimeSpan? delay = null)
    {
        var actualDelay = delay ?? TimeSpan.FromSeconds(1);
        var attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (IsTransientException(ex) && attempts < maxRetries - 1)
            {
                attempts++;
                await Task.Delay(actualDelay);
                actualDelay = TimeSpan.FromMilliseconds(actualDelay.TotalMilliseconds * 1.5); // Exponential backoff
            }
        }

        return await operation(); // Final attempt without catching exceptions
    }

    /// <summary>
    /// Checks if an exception is transient and worth retrying.
    /// </summary>
    private static bool IsTransientException(Exception exception)
    {
        return exception is TimeoutException ||
               exception.Message.Contains("timeout") ||
               exception.Message.Contains("connection");
    }

    /// <summary>
    /// Builds a dynamic order by expression from a property name.
    /// </summary>
    public static IQueryable<T> OrderByProperty<T>(
        this IQueryable<T> source, 
        string propertyName, 
        bool descending = false)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, propertyName);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = descending ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), property.Type },
            source.Expression,
            Expression.Quote(lambda));

        return source.Provider.CreateQuery<T>(resultExpression);
    }
}
```

### Data Validation Utils

```csharp
namespace Database.Utils;

/// <summary>
/// Provides validation utilities for data integrity checks.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates that required properties are not null or empty.
    /// </summary>
    public static ValidationResult ValidateRequiredFields<T>(T entity, params Expression<Func<T, object>>[] properties)
    {
        var errors = new List<string>();

        foreach (var property in properties)
        {
            var memberExpression = GetMemberExpression(property);
            var propertyName = memberExpression.Member.Name;
            var value = property.Compile()(entity);

            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                errors.Add($"{propertyName} is required.");
            }
        }

        return new ValidationResult(errors.Count == 0, errors);
    }

    /// <summary>
    /// Validates email format.
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
            return memberExpression;

        if (expression.Body is UnaryExpression unaryExpression && 
            unaryExpression.Operand is MemberExpression operand)
            return operand;

        throw new ArgumentException("Expression must be a member expression");
    }
}

public record class ValidationResult(bool IsValid, List<string> Errors);
```

### Configuration Utils

```csharp
namespace Database.Utils;

/// <summary>
/// Utilities for database configuration and connection management.
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    /// Builds a connection string from individual components.
    /// </summary>
    public static string BuildConnectionString(
        string server,
        string database,
        string? userId = null,
        string? password = null,
        bool integratedSecurity = true,
        int commandTimeout = 30)
    {
        var builder = new StringBuilder();
        builder.Append($"Server={server};");
        builder.Append($"Database={database};");

        if (integratedSecurity)
        {
            builder.Append("Integrated Security=true;");
        }
        else
        {
            builder.Append($"User Id={userId};");
            builder.Append($"Password={password};");
        }

        builder.Append($"Command Timeout={commandTimeout};");
        builder.Append("TrustServerCertificate=true;");

        return builder.ToString();
    }

    /// <summary>
    /// Configures DbContext options with standard settings.
    /// </summary>
    public static void ConfigureDbContextOptions(
        DbContextOptionsBuilder options,
        string connectionString,
        ILoggerFactory? loggerFactory = null)
    {
        options.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });

        if (loggerFactory != null)
        {
            options.UseLoggerFactory(loggerFactory);
        }

        options.EnableSensitiveDataLogging(false);
        options.EnableDetailedErrors(true);
    }
}
```

### Query Building Utils

```csharp
namespace Database.Utils;

/// <summary>
/// Utilities for building complex database queries.
/// </summary>
public static class QueryBuilder
{
    /// <summary>
    /// Applies pagination to a query with total count.
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }

    /// <summary>
    /// Applies keyset pagination for efficient large dataset handling.
    /// </summary>
    public static IQueryable<T> ApplyKeysetPagination<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> keySelector,
        TKey? lastKey,
        int pageSize,
        bool ascending = true)
        where TKey : IComparable<TKey>
    {
        if (lastKey != null)
        {
            if (ascending)
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(
                    Expression.GreaterThan(keySelector.Body,
                        Expression.Constant(lastKey, typeof(TKey))),
                    keySelector.Parameters));
            }
            else
            {
                query = query.Where(Expression.Lambda<Func<T, bool>>(
                    Expression.LessThan(keySelector.Body,
                        Expression.Constant(lastKey, typeof(TKey))),
                    keySelector.Parameters));
            }
        }

        return ascending 
            ? query.OrderBy(keySelector).Take(pageSize)
            : query.OrderByDescending(keySelector).Take(pageSize);
    }

    /// <summary>
    /// Applies dynamic filtering based on search criteria.
    /// </summary>
    public static IQueryable<T> ApplySearch<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string>>[] searchProperties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !searchProperties.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var property in searchProperties)
        {
            var propertyExpression = Expression.Invoke(property, parameter);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var containsExpression = Expression.Call(
                propertyExpression,
                containsMethod!,
                Expression.Constant(searchTerm));

            combinedExpression = combinedExpression == null
                ? containsExpression
                : Expression.OrElse(combinedExpression, containsExpression);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression!, parameter);
        return query.Where(lambda);
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
```

### Audit Utils

```csharp
namespace Database.Utils;

/// <summary>
/// Utilities for audit trail and change tracking.
/// </summary>
public static class AuditHelper
{
    /// <summary>
    /// Automatically sets audit fields on entities that implement IAuditable.
    /// </summary>
    public static void SetAuditFields(DbContext context, Guid? userId = null)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is IAuditable && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var auditable = (IAuditable)entry.Entity;
            var now = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                auditable.CreatedAt = now;
                auditable.CreatedBy = userId;
            }

            auditable.UpdatedAt = now;
            auditable.UpdatedBy = userId;
        }
    }

    /// <summary>
    /// Creates an audit log entry for entity changes.
    /// </summary>
    public static AuditLogEntry CreateAuditEntry(EntityEntry entry, Guid? userId = null)
    {
        return new AuditLogEntry
        {
            EntityName = entry.Entity.GetType().Name,
            EntityId = GetEntityId(entry),
            Action = entry.State.ToString(),
            Changes = GetChanges(entry),
            UserId = userId,
            Timestamp = DateTime.UtcNow
        };
    }

    private static string GetEntityId(EntityEntry entry)
    {
        var keyValues = entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .Select(p => p.CurrentValue?.ToString())
            .Where(v => v != null);

        return string.Join(",", keyValues);
    }

    private static Dictionary<string, object?> GetChanges(EntityEntry entry)
    {
        var changes = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.IsModified)
            {
                changes[property.Metadata.Name] = new
                {
                    OldValue = property.OriginalValue,
                    NewValue = property.CurrentValue
                };
            }
        }

        return changes;
    }
}

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    Guid? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
}

public record class AuditLogEntry
{
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Dictionary<string, object?> Changes { get; set; } = [];
    public Guid? UserId { get; set; }
    public DateTime Timestamp { get; set; }
}
```

## Best Practices

### Utility Design

1. **Static classes**: Use static classes for stateless utility methods
2. **Single responsibility**: Each utility class should have a focused purpose
3. **Generic methods**: Create generic versions where applicable
4. **Error handling**: Include appropriate exception handling

### Performance Considerations

1. **Async operations**: Use async/await for database operations
2. **Memory efficiency**: Avoid loading large datasets into memory
3. **Query optimization**: Ensure utilities generate efficient SQL
4. **Caching**: Consider caching for frequently used computations

### Testing

1. **Unit testable**: Design utilities to be easily unit tested
2. **Dependency injection**: Accept dependencies through parameters when needed
3. **Mocking**: Ensure database-dependent utilities can be mocked

### Documentation

1. **XML comments**: Include comprehensive documentation
2. **Usage examples**: Provide code examples in documentation
3. **Parameter validation**: Document parameter requirements and constraints

Utilities provide essential support functionality that makes the rest of your database layer more maintainable and efficient while avoiding code duplication across the application.
