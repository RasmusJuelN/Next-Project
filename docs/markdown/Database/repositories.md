# Repository Pattern Guide

This guide provides comprehensive documentation for implementing and using the Repository pattern in the Backend project's Database layer.

## Overview

The Repository pattern provides an abstraction layer over data access operations, promoting separation of concerns and testability. This project implements a generic repository base with specialized repositories for specific entities.

## Architecture

### Generic Repository Foundation

The `GenericRepository<TEntity>` class provides common CRUD operations that can be used by all entity types:

- **CRUD Operations**: Add, Get, Update, Delete with async support
- **LINQ Support**: Expression-based querying with predicate filters
- **Query Modification**: Flexible query customization for includes, ordering, and filtering
- **Performance Optimization**: Count and existence checking without entity materialization

### Specialized Repositories

Entity-specific repositories extend the generic repository to provide business logic and domain-specific operations:

- **`UserRepository`**: Handles user management with TPH inheritance support
- **`ActiveQuestionnaireRepository`**: Manages questionnaire lifecycle and complex filtering
- **`QuestionnaireTemplateRepository`**: Template operations and validation
- **`ApplicationLogRepository`**: Logging and audit trail management
- **`TrackedRefreshTokenRepository`**: Authentication token management

## Existing Repositories

### UserRepository

Handles user operations across the TPH inheritance hierarchy:

```csharp
// Role-specific queries
Task<FullUser?> GetStudentAsync(Guid id);
Task<FullUser?> GetTeacherAsync(Guid id);

// Universal user query
Task<FullUser?> GetUserAsync(Guid id);

// Business logic operations
Task<Guid?> GetIdOfOldestActiveQuestionnaire(Guid id);
```

**Key Features:**
- TPH inheritance support with `OfType<T>()` filtering
- Role-specific data access methods
- Workflow management through questionnaire tracking
- Efficient existence checking

### ActiveQuestionnaireRepository

Manages complex questionnaire operations with extensive filtering and pagination:

```csharp
// Basic retrieval
Task<ActiveQuestionnaireBase> GetActiveQuestionnaireBaseAsync(Guid id);
Task<ActiveQuestionnaire> GetActiveQuestionnaireAsync(Guid id);

// Advanced querying with pagination
Task<ActiveQuestionnaireKeysetPaginationResponse> GetActiveQuestionnairesAsync(/* pagination params */);
```

**Key Features:**
- Keyset pagination for large datasets
- Complex filtering with multiple criteria
- Eager loading of related entities
- Response tracking and completion status

### GenericRepository

Provides the foundation for all repository operations:

```csharp
// Query operations
Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, 
                             Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryModifier = null);
Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);

// CRUD operations
Task AddAsync(TEntity entity);
void Delete(TEntity entity);

// Utility operations
int Count(Expression<Func<TEntity, bool>>? predicate = null);
bool Exists(Expression<Func<TEntity, bool>> predicate);
```

## Creating a Repository

### 1. Define the Interface

Create an interface that defines the contract for your repository:

```csharp
using Database.DTO.YourEntity;

namespace Database.Interfaces;

/// <summary>
/// Defines the contract for YourEntity repository operations.
/// Provides methods for entity-specific business logic and data access.
/// </summary>
public interface IYourEntityRepository
{
    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The entity DTO or null if not found.</returns>
    Task<YourEntityDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Retrieves entities based on specific business criteria.
    /// </summary>
    /// <param name="criteria">The business-specific filter criteria.</param>
    /// <returns>A list of entities matching the criteria.</returns>
    Task<List<YourEntityDto>> GetByCriteriaAsync(YourEntityCriteria criteria);

    /// <summary>
    /// Creates a new entity with business validation.
    /// </summary>
    /// <param name="createDto">The data for creating the entity.</param>
    /// <returns>The created entity DTO.</returns>
    Task<YourEntityDto> CreateAsync(CreateYourEntityDto createDto);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update.</param>
    /// <param name="updateDto">The update data.</param>
    /// <returns>The updated entity DTO.</returns>
    Task<YourEntityDto> UpdateAsync(Guid id, UpdateYourEntityDto updateDto);

    /// <summary>
    /// Soft deletes an entity (if applicable) or hard deletes.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Checks if an entity exists with the given criteria.
    /// </summary>
    /// <param name="predicate">The existence criteria.</param>
    /// <returns>True if the entity exists, false otherwise.</returns>
    Task<bool> ExistsAsync(Expression<Func<YourEntityModel, bool>> predicate);
}
```

### 2. Implement the Repository

```csharp
using Database.DTO.YourEntity;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

/// <summary>
/// Implements repository operations for YourEntity management.
/// Provides business logic and data access operations specific to YourEntity.
/// </summary>
/// <param name="context">The database context for data access operations.</param>
/// <param name="loggerFactory">Factory for creating loggers for diagnostic purposes.</param>
public class YourEntityRepository(Context context, ILoggerFactory loggerFactory) : IYourEntityRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<YourEntityModel> _genericRepository = new(context, loggerFactory);
    private readonly ILogger<YourEntityRepository> _logger = loggerFactory.CreateLogger<YourEntityRepository>();

    public async Task<YourEntityDto?> GetByIdAsync(Guid id)
    {
        var entity = await _genericRepository.GetSingleAsync(
            e => e.Id == id,
            query => query.Include(e => e.RelatedEntities) // Include related data
        );
        
        return entity?.ToDto();
    }

    public async Task<List<YourEntityDto>> GetByCriteriaAsync(YourEntityCriteria criteria)
    {
        var entities = await _genericRepository.GetAsync(
            e => e.Name.Contains(criteria.SearchTerm) && 
                 e.CreatedAt >= criteria.FromDate &&
                 e.Status == criteria.Status,
            query => query
                .Include(e => e.RelatedEntities)
                .OrderByDescending(e => e.CreatedAt)
                .Take(criteria.MaxResults)
        );
        
        return entities.Select(e => e.ToDto()).ToList();
    }

    public async Task<YourEntityDto> CreateAsync(CreateYourEntityDto createDto)
    {
        // Business validation
        var existingEntity = await _genericRepository.GetSingleAsync(e => e.Name == createDto.Name);
        if (existingEntity != null)
        {
            throw new InvalidOperationException($"Entity with name '{createDto.Name}' already exists.");
        }

        // Create entity
        var entity = new YourEntityModel
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            CreatedAt = DateTime.UtcNow,
            Status = EntityStatus.Active
        };

        await _genericRepository.AddAsync(entity);
        
        _logger.LogInformation("Created new entity with ID: {EntityId}", entity.Id);
        
        return entity.ToDto();
    }

    public async Task<YourEntityDto> UpdateAsync(Guid id, UpdateYourEntityDto updateDto)
    {
        var entity = await _genericRepository.GetSingleAsync(e => e.Id == id);
        if (entity == null)
        {
            throw new EntityNotFoundException($"Entity with ID {id} not found.");
        }

        // Update properties
        entity.Name = updateDto.Name;
        entity.Description = updateDto.Description;
        entity.LastUpdated = DateTime.UtcNow;

        _logger.LogInformation("Updated entity with ID: {EntityId}", entity.Id);
        
        return entity.ToDto();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _genericRepository.GetSingleAsync(e => e.Id == id);
        if (entity == null)
        {
            throw new EntityNotFoundException($"Entity with ID {id} not found.");
        }

        _genericRepository.Delete(entity);
        
        _logger.LogInformation("Deleted entity with ID: {EntityId}", id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<YourEntityModel, bool>> predicate)
    {
        return await _genericRepository.ExistsAsync(predicate);
    }
}
```

## Common LINQ Usage and Syntax

### Basic Filtering

```csharp
// Simple equality
var users = await _genericRepository.GetAsync(u => u.IsActive == true);

// Multiple conditions with AND
var activeUsers = await _genericRepository.GetAsync(u => 
    u.IsActive && u.CreatedAt >= DateTime.Today.AddDays(-30));

// OR conditions
var recentOrVipUsers = await _genericRepository.GetAsync(u => 
    u.CreatedAt >= DateTime.Today.AddDays(-7) || u.IsVip);

// String operations
var searchResults = await _genericRepository.GetAsync(u => 
    u.Name.Contains(searchTerm) || u.Email.StartsWith(emailPrefix));

// Numeric ranges
var expensiveProducts = await _genericRepository.GetAsync(p => 
    p.Price >= 100.00m && p.Price <= 1000.00m);
```

### Collection Operations

```csharp
// Check if property is in a list
var validStatuses = new[] { Status.Active, Status.Pending };
var entitiesWithValidStatus = await _genericRepository.GetAsync(e => 
    validStatuses.Contains(e.Status));

// Any/All operations (requires queryModifier)
var entitiesWithRelatedData = await _genericRepository.GetAsync(e => e.Id != Guid.Empty,
    query => query.Where(e => e.RelatedEntities.Any(r => r.IsImportant)));
```

### Date and Time Operations

```csharp
// Date comparisons
var today = DateTime.Today;
var todaysEntities = await _genericRepository.GetAsync(e => 
    e.CreatedAt >= today && e.CreatedAt < today.AddDays(1));

// Date parts
var currentYearEntities = await _genericRepository.GetAsync(e => 
    e.CreatedAt.Year == DateTime.Now.Year);

// Date ranges
var lastWeekEntities = await _genericRepository.GetAsync(e => 
    e.CreatedAt >= DateTime.Today.AddDays(-7));
```

### Sorting and Ordering

```csharp
// Single property ordering
var orderedEntities = await _genericRepository.GetAsync(e => e.IsActive,
    query => query.OrderBy(e => e.Name));

// Multiple property ordering
var complexOrdering = await _genericRepository.GetAsync(e => e.IsActive,
    query => query
        .OrderByDescending(e => e.Priority)
        .ThenBy(e => e.CreatedAt)
        .ThenBy(e => e.Name));
```

### Pagination

```csharp
// Skip/Take pagination
var pagedResults = await _genericRepository.GetAsync(e => e.IsActive,
    query => query
        .OrderBy(e => e.CreatedAt)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize));

// Keyset pagination (more efficient for large datasets)
var keysetResults = await _genericRepository.GetAsync(
    e => e.CreatedAt > lastSeenDate && e.IsActive,
    query => query
        .OrderBy(e => e.CreatedAt)
        .Take(pageSize));
```

## Handling Relationships and Includes

### Eager Loading with Include

```csharp
// Single navigation property
var usersWithQuestionnaires = await _genericRepository.GetAsync(u => u.IsActive,
    query => query.Include(u => u.ActiveQuestionnaires));

// Multiple navigation properties
var fullUserData = await _genericRepository.GetAsync(u => u.IsActive,
    query => query
        .Include(u => u.ActiveQuestionnaires)
        .Include(u => u.TrackedRefreshTokens));

// Nested includes (ThenInclude)
var questionnaireWithDetails = await _genericRepository.GetAsync(q => q.IsActive,
    query => query
        .Include(q => q.Questions)
            .ThenInclude(qu => qu.Options)
        .Include(q => q.ActiveQuestionnaires)
            .ThenInclude(aq => aq.Student));

// Filtered includes (Entity Framework 5.0+)
var usersWithRecentQuestionnaires = await _genericRepository.GetAsync(u => u.IsActive,
    query => query
        .Include(u => u.ActiveQuestionnaires.Where(aq => aq.CreatedAt >= DateTime.Today.AddDays(-30))));
```

### Projection for Performance

```csharp
// Select specific properties to avoid loading full entities
var userSummaries = await _context.Users
    .Where(u => u.IsActive)
    .Select(u => new UserSummaryDto
    {
        Id = u.Id,
        Name = u.FullName,
        QuestionnaireCount = u.ActiveQuestionnaires.Count()
    })
    .ToListAsync();

// Anonymous projections for internal use
var entityCounts = await _context.QuestionnaireTemplates
    .GroupBy(q => q.Category)
    .Select(g => new { Category = g.Key, Count = g.Count() })
    .ToListAsync();
```

### Complex Join Operations

```csharp
// Explicit joins using LINQ syntax
var questionnaireSummary = await (from template in _context.QuestionnaireTemplates
                                  join active in _context.ActiveQuestionnaires 
                                      on template.Id equals active.QuestionnaireTemplateFK
                                  join student in _context.Users.OfType<StudentModel>() 
                                      on active.StudentFK equals student.Id
                                  where active.StudentCompletedAt == null
                                  select new QuestionnaireAssignmentDto
                                  {
                                      TemplateTitle = template.Title,
                                      StudentName = student.FullName,
                                      AssignedDate = active.ActivatedAt
                                  }).ToListAsync();
```

## Handling TPH (Table Per Hierarchy) Models

### Querying Specific Types

```csharp
// Query only students
var students = await _context.Users.OfType<StudentModel>()
    .Where(s => s.IsActive)
    .ToListAsync();

// Query only teachers
var teachers = await _context.Users.OfType<TeacherModel>()
    .Where(t => t.Department == "Computer Science")
    .ToListAsync();

// Query base type (includes all derived types)
var allUsers = await _context.Users
    .Where(u => u.CreatedAt >= DateTime.Today.AddDays(-30))
    .ToListAsync();
```

### Type-Specific Operations in Repository

```csharp
public class UserRepository : IUserRepository
{
    public async Task<List<StudentDto>> GetActiveStudentsAsync()
    {
        var students = await _context.Users.OfType<StudentModel>()
            .Where(s => s.IsActive)
            .Include(s => s.ActiveQuestionnaires)
            .ToListAsync();
            
        return students.Select(s => s.ToStudentDto()).ToList();
    }

    public async Task<List<TeacherDto>> GetTeachersByDepartmentAsync(string department)
    {
        var teachers = await _context.Users.OfType<TeacherModel>()
            .Where(t => t.Department == department && t.IsActive)
            .Include(t => t.ActiveQuestionnaires)
            .ToListAsync();
            
        return teachers.Select(t => t.ToTeacherDto()).ToList();
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        // This will work for any user type (Student or Teacher)
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Guid == id);
            
        return user?.ToUserDto();
    }
}
```

### Polymorphic Queries

```csharp
// Query that works across all user types
var usersWithPendingQuestionnaires = await _context.Users
    .Where(u => u.ActiveQuestionnaires.Any(aq => 
        (u is StudentModel && !aq.StudentCompletedAt.HasValue) ||
        (u is TeacherModel && !aq.TeacherCompletedAt.HasValue)))
    .ToListAsync();

// Role-based filtering using discriminator
var usersByRole = await _context.Users
    .Where(u => u.PrimaryRole == UserRoles.Student)
    .ToListAsync();
```

### Generic Repository with TPH

```csharp
public class GenericUserRepository<TUser> where TUser : UserBaseModel
{
    private readonly Context _context;

    public async Task<List<TUser>> GetActiveUsersAsync()
    {
        return await _context.Users.OfType<TUser>()
            .Where(u => u.IsActive)
            .ToListAsync();
    }

    public async Task<TUser?> GetByIdAsync(Guid id)
    {
        return await _context.Users.OfType<TUser>()
            .FirstOrDefaultAsync(u => u.Guid == id);
    }
}

// Usage
var studentRepo = new GenericUserRepository<StudentModel>();
var teacherRepo = new GenericUserRepository<TeacherModel>();
```

## Best Practices

### Repository Design

1. **Single Responsibility**: Each repository should handle one aggregate root
2. **Interface Segregation**: Define focused interfaces for specific operations
3. **Dependency Injection**: Use constructor injection for dependencies
4. **Async Operations**: Use async/await for all database operations
5. **Error Handling**: Implement proper exception handling and logging

### Query Optimization

1. **Projection**: Use `Select()` to load only needed properties
2. **Filtering**: Apply `Where()` clauses as early as possible
3. **Includes**: Be selective with `Include()` to avoid N+1 problems
4. **Pagination**: Implement proper pagination for large datasets
5. **Indexing**: Ensure database indexes support your query patterns

### Performance Considerations

```csharp
// Good: Efficient projection
var summaries = await _context.Users
    .Where(u => u.IsActive)
    .Select(u => new { u.Id, u.Name, QuestionnaireCount = u.ActiveQuestionnaires.Count() })
    .ToListAsync();

// Bad: Loading full entities when only summary is needed
var users = await _context.Users
    .Include(u => u.ActiveQuestionnaires)
    .Where(u => u.IsActive)
    .ToListAsync();
// This approach is for demonstration only and is not recommended for production use due to performance issues with loading full entities.
var summaries = users.Select(u => new { u.Id, u.Name, QuestionnaireCount = u.ActiveQuestionnaires.Count });
```

### Error Handling

```csharp
public async Task<UserDto> GetUserAsync(Guid id)
{
    try
    {
        var user = await _genericRepository.GetSingleAsync(u => u.Guid == id);
        
        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            throw new EntityNotFoundException($"User with ID {id} not found");
        }
        
        return user.ToDto();
    }
    catch (InvalidOperationException ex)
    {
        _logger.LogError(ex, "Multiple users found with ID: {UserId}", id);
        throw new DataIntegrityException("Multiple users found with the same ID", ex);
    }
}
```

## Integration with Unit of Work

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly Context _context;
    
    public IUserRepository Users { get; }
    public IActiveQuestionnaireRepository ActiveQuestionnaires { get; }
    public IQuestionnaireTemplateRepository Templates { get; }

    public UnitOfWork(Context context, ILoggerFactory loggerFactory)
    {
        _context = context;
        Users = new UserRepository(context, loggerFactory);
        ActiveQuestionnaires = new ActiveQuestionnaireRepository(context, loggerFactory);
        Templates = new QuestionnaireTemplateRepository(context, loggerFactory);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

## Common Pitfalls and Anti-Patterns

### Repository Anti-Patterns

#### 1. Leaky Abstractions

**Bad Example:**
```csharp
public interface IUserRepository
{
    // Exposes Entity Framework specific types
    Task<IQueryable<UserModel>> GetUsersQueryable();
    Task<DbSet<UserModel>> GetDbSet();
}
```

**Good Example:**
```csharp
public interface IUserRepository
{
    // Clean abstraction with domain-specific methods
    Task<List<UserDto>> GetActiveUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
}
```

#### 2. Generic Repository Over-Abstraction

**Bad Example:**
```csharp
// Too generic - loses domain meaning
public interface IGenericRepository<T>
{
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<IQueryable<T>> GetAllAsync();
}

// Forces consumers to know about implementation details
var users = await _genericRepo.GetAllAsync()
    .Where(u => u.IsActive && u.Role == "Student")
    .Include(u => u.Profile)
    .ToListAsync();
```

**Good Example:**
```csharp
// Domain-specific interface
public interface IUserRepository
{
    Task<UserDto> CreateUserAsync(CreateUserDto createDto);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateDto);
    Task DeleteUserAsync(Guid id);
    Task<List<StudentDto>> GetActiveStudentsAsync();
}
```

#### 3. N+1 Query Problems

**Bad Example:**
```csharp
public async Task<List<UserSummaryDto>> GetUserSummariesAsync()
{
    var users = await _context.Users.ToListAsync();
    var summaries = new List<UserSummaryDto>();
    
    foreach (var user in users)
    {
        // This causes N+1 queries!
        var questionnaireCount = await _context.ActiveQuestionnaires
            .CountAsync(q => q.StudentFK == user.Id);
            
        summaries.Add(new UserSummaryDto
        {
            Id = user.Id,
            Name = user.FullName,
            QuestionnaireCount = questionnaireCount
        });
    }
    
    return summaries;
}
```

**Good Example:**
```csharp
public async Task<List<UserSummaryDto>> GetUserSummariesAsync()
{
    return await _context.Users
        .Select(u => new UserSummaryDto
        {
            Id = u.Id,
            Name = u.FullName,
            QuestionnaireCount = u.ActiveQuestionnaires.Count()
        })
        .ToListAsync();
}
```
>[!NOTE]
><details>
><summary> Click to expand for explanation </summary>
>
>* The bad example retrieves all users and then iterates over them to count related questionnaires, resulting in N+1 queries (one for users and one for each user to count questionnaires).
>* The good example uses a single query with projection to retrieve users along with their questionnaire counts, avoiding the N+1 problem and improving performance.
></details>

#### 4. Inappropriate Use of Include

**Bad Example:**
```csharp
// Loads unnecessary data for simple count operation
public async Task<int> GetUserQuestionnaireCountAsync(Guid userId)
{
    var user = await _context.Users
        .Include(u => u.ActiveQuestionnaires)
            .ThenInclude(q => q.Template)
                .ThenInclude(t => t.Questions)
        .FirstOrDefaultAsync(u => u.Id == userId);
        
    return user?.ActiveQuestionnaires.Count() ?? 0;
}
```

**Good Example:**
```csharp
// Efficient count without loading entities
public async Task<int> GetUserQuestionnaireCountAsync(Guid userId)
{
    return await _context.ActiveQuestionnaires
        .CountAsync(q => q.StudentFK == userId);
}
```

### Query Performance Anti-Patterns

#### 1. Client-Side Evaluation

**Bad Example:**
```csharp
// Complex business logic evaluated client-side
var eligibleUsers = await _context.Users
    .ToListAsync() // Brings all users to memory
    .Where(u => IsUserEligibleForPremium(u)) // Client-side evaluation
    .ToList();

private bool IsUserEligibleForPremium(UserModel user)
{
    // Complex business logic
    return user.RegistrationDate <= DateTime.Now.AddMonths(-6) &&
           user.ActiveQuestionnaires.Count() >= 5;
}
```

**Good Example:**
```csharp
// Server-side evaluation with translatable expressions
var sixMonthsAgo = DateTime.Now.AddMonths(-6);
var eligibleUsers = await _context.Users
    .Where(u => u.RegistrationDate <= sixMonthsAgo &&
                u.ActiveQuestionnaires.Count() >= 5)
    .ToListAsync();
```

#### 2. Unnecessary Entity Tracking

**Bad Example:**
```csharp
// Tracking entities for read-only operations
public async Task<List<UserSummaryDto>> GetUserSummariesAsync()
{
    var users = await _context.Users
        .Include(u => u.ActiveQuestionnaires)
        .ToListAsync(); // Entities are tracked
        
    return users.Select(u => u.ToSummaryDto()).ToList();
}
```

**Good Example:**
```csharp
// Disable tracking for read-only operations
public async Task<List<UserSummaryDto>> GetUserSummariesAsync()
{
    return await _context.Users
        .AsNoTracking() // Improves performance for read-only scenarios
        .Select(u => new UserSummaryDto
        {
            Id = u.Id,
            Name = u.FullName,
            QuestionnaireCount = u.ActiveQuestionnaires.Count()
        })
        .ToListAsync();
}
```

#### 3. Inefficient Pagination

**Bad Example:**
```csharp
// Skip becomes inefficient with large offsets
public async Task<List<UserDto>> GetUsersPageAsync(int page, int pageSize)
{
    var skip = (page - 1) * pageSize;
    
    return await _context.Users
        .OrderBy(u => u.Id) // Non-selective ordering
        .Skip(skip) // Inefficient for large offsets
        .Take(pageSize)
        .Select(u => u.ToDto())
        .ToListAsync();
}
```

**Good Example:**
```csharp
// Keyset pagination with selective ordering
public async Task<List<UserDto>> GetUsersPageAsync(DateTime? lastCreatedAt = null, int pageSize = 20)
{
    var query = _context.Users.AsQueryable();
    
    if (lastCreatedAt.HasValue)
    {
        query = query.Where(u => u.CreatedAt > lastCreatedAt.Value);
    }
    
    return await query
        .OrderBy(u => u.CreatedAt) // Indexed column
        .Take(pageSize)
        .Select(u => u.ToDto())
        .ToListAsync();
}
```

### Error Handling Anti-Patterns

#### 1. Swallowing Exceptions

**Bad Example:**
```csharp
public async Task<UserDto?> GetUserAsync(Guid id)
{
    try
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user?.ToDto();
    }
    catch
    {
        // Swallowing exceptions without logging or handling
        return null;
    }
}
```

**Good Example:**
```csharp
public async Task<UserDto?> GetUserAsync(Guid id)
{
    try
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        return user?.ToDto();
    }
    catch (SqlException ex)
    {
        _logger.LogError(ex, "Database error occurred while retrieving user {UserId}", id);
        throw new DataAccessException("Failed to retrieve user", ex);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error occurred while retrieving user {UserId}", id);
        throw;
    }
}
```

#### 2. Repository as Catch-All

**Bad Example:**
```csharp
public class UserRepository : IUserRepository
{
    // Too many responsibilities
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Business validation
        ValidateUserData(dto);
        
        // Email sending
        await _emailService.SendWelcomeEmailAsync(dto.Email);
        
        // Audit logging
        await _auditService.LogUserCreationAsync(dto);
        
        // File system operations
        await _fileService.CreateUserDirectoryAsync(dto.Username);
        
        // Database operation
        var user = new UserModel { /* ... */ };
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return user.ToDto();
    }
}
```

**Good Example:**
```csharp
public class UserRepository : IUserRepository
{
    // Focused on data access only
    public async Task<UserDto> CreateUserAsync(UserModel user)
    {
        await _context.Users.AddAsync(user);
        return user.ToDto();
    }
}

// Business logic handled in service layer
public class UserService : IUserService
{
    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        // Validation
        await _validationService.ValidateUserDataAsync(dto);
        
        // Create user entity
        var user = _mapper.Map<UserModel>(dto);
        var createdUser = await _userRepository.CreateUserAsync(user);
        await _unitOfWork.SaveChangesAsync();
        
        // Side effects
        await _emailService.SendWelcomeEmailAsync(dto.Email);
        await _auditService.LogUserCreationAsync(createdUser.Id);
        
        return createdUser;
    }
}
```

### Testing Anti-Patterns

#### 1. Testing Repository Implementation Instead of Interface

**Bad Example:**
```csharp
[Test]
public async Task UserRepository_Should_Use_Correct_Include_Statements()
{
    // Testing implementation details
    var repository = new UserRepository(_context, _loggerFactory);
    var users = await repository.GetUsersWithQuestionnairesAsync();
    
    // This test is brittle and couples to implementation
    _context.Entry(users.First())
        .Collection(u => u.ActiveQuestionnaires)
        .IsLoaded.Should().BeTrue();
}
```

**Good Example:**
```csharp
[Test]
public async Task GetUsersWithQuestionnaires_Should_Return_Users_With_Questionnaire_Data()
{
    // Testing behavior through interface
    var users = await _userRepository.GetUsersWithQuestionnairesAsync();
    
    users.Should().NotBeEmpty();
    users.First().Questionnaires.Should().NotBeNull();
    users.First().Questionnaires.Should().NotBeEmpty();
}
```

#### 2. Over-Mocking in Repository Tests

**Bad Example:**
```csharp
[Test]
public async Task GetUser_Should_Return_User()
{
    // Over-mocking EF Core components
    var mockSet = new Mock<DbSet<UserModel>>();
    var mockContext = new Mock<Context>();
    mockSet.Setup(m => m.FindAsync(It.IsAny<Guid>()))
           .ReturnsAsync(new UserModel { Id = Guid.NewGuid() });
    mockContext.Setup(c => c.Users).Returns(mockSet.Object);
    
    var repository = new UserRepository(mockContext.Object, _loggerFactory);
    var result = await repository.GetUserAsync(Guid.NewGuid());
    
    result.Should().NotBeNull();
}
```

**Good Example:**
```csharp
[Test]
public async Task GetUser_Should_Return_User()
{
    // Use in-memory database for integration testing
    var options = new DbContextOptionsBuilder<Context>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
        
    await using var context = new Context(options);
    var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User" };
    context.Users.Add(user);
    await context.SaveChangesAsync();
    
    var repository = new UserRepository(context, _loggerFactory);
    var result = await repository.GetUserAsync(user.Id);
    
    result.Should().NotBeNull();
    result.Name.Should().Be("Test User");
}
```

### Common Performance Pitfalls

1. **Loading Full Entities for Count Operations**: Use `CountAsync()` instead of `ToListAsync().Count()`
2. **Unnecessary Includes**: Only include related data that will be used
3. **Blocking Async Calls**: Avoid `.Result` or `.Wait()` on async operations
4. **Large Dataset Operations**: Implement proper pagination and filtering
5. **Missing Database Indexes**: Ensure queries are supported by appropriate indexes
6. **Client-Side Grouping**: Use SQL grouping with `GroupBy()` instead of in-memory grouping

### Memory Management Issues

```csharp
// Bad: Memory leak with large datasets
public async Task<List<UserDto>> GetAllUsersAsync()
{
    var users = await _context.Users
        .Include(u => u.ActiveQuestionnaires)
            .ThenInclude(q => q.Template)
                .ThenInclude(t => t.Questions)
        .ToListAsync(); // Loads everything into memory
        
    return users.Select(u => u.ToDto()).ToList();
}

// Good: Streaming with projection
public async IAsyncEnumerable<UserDto> GetAllUsersStreamAsync()
{
    await foreach (var user in _context.Users
        .AsNoTracking()
        .Select(u => new UserDto { Id = u.Id, Name = u.FullName })
        .AsAsyncEnumerable())
    {
        yield return user;
    }
}
```

By avoiding these common pitfalls and anti-patterns, you can create more maintainable, performant, and testable repository implementations that properly abstract data access concerns while leveraging Entity Framework Core's capabilities effectively.

This guide provides a comprehensive foundation for implementing and using the Repository pattern in the Backend project. The pattern promotes clean architecture, testability, and maintainable data access code while leveraging Entity Framework Core's powerful querying capabilities.
