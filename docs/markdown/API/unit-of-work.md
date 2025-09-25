# Unit of Work Pattern

The Unit of Work pattern is a fundamental design pattern used in the NextQuestionnaire Backend to manage database operations and maintain data consistency. This guide explains what the Unit of Work pattern is, why it's used, and how to work with it effectively.

## What is the Unit of Work Pattern?

The Unit of Work pattern maintains a list of objects affected by a business transaction and coordinates writing out changes and resolving concurrency problems. It acts as a coordinator between your business logic and data access layers, ensuring that all related operations either succeed or fail together.

### Key Concepts

- **Transaction Management**: Ensures atomicity across multiple repository operations
- **Change Tracking**: Coordinates Entity Framework change detection and saving
- **Resource Management**: Proper disposal of database connections and transactions
- **Repository Coordination**: Central access point for all data access repositories

## Why Use Unit of Work?

### 1. **Data Consistency**
The Unit of Work ensures that all related operations are performed as a single atomic transaction. If any operation fails, all changes can be rolled back, maintaining data integrity.

```csharp
// Example: Creating a questionnaire that involves multiple repositories
await _unitOfWork.BeginTransactionAsync();
try 
{
    // Add student if doesn't exist
    if (!_unitOfWork.User.UserExists(studentId))
        await _unitOfWork.User.AddStudentAsync(student);
    
    // Add teacher if doesn't exist
    if (!_unitOfWork.User.UserExists(teacherId))
        await _unitOfWork.User.AddTeacherAsync(teacher);
    
    // Create the questionnaire
    var questionnaire = await _unitOfWork.ActiveQuestionnaire.ActivateQuestionnaireAsync(templateId, studentId, teacherId);
    
    // Save all changes together
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitAsync();
}
catch (Exception)
{
    await _unitOfWork.RollbackAsync();
    throw;
}
```

### 2. **Reduced Database Round Trips**
By batching operations and saving changes once at the end, the Unit of Work reduces the number of database round trips, improving performance.

### 3. **Centralized Transaction Management**
All database operations go through a single point of control, making it easier to manage transactions and ensure consistency.

### 4. **Testability**
The Unit of Work pattern makes it easier to mock database operations for unit testing, as you only need to mock one interface rather than multiple repository interfaces.

## The IUnitOfWork Interface

The NextQuestionnaire Backend defines the Unit of Work contract through the `IUnitOfWork` interface:

```csharp
public interface IUnitOfWork : IDisposable
{
    // Repository Properties
    IQuestionnaireTemplateRepository QuestionnaireTemplate { get; }
    IActiveQuestionnaireRepository ActiveQuestionnaire { get; }
    IUserRepository User { get; }
    ITrackedRefreshTokenRepository TrackedRefreshToken { get; }
    
    // Transaction Management
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}
```

### Repository Properties

The Unit of Work provides access to all the repositories in the system:

- **QuestionnaireTemplate**: Manages questionnaire templates and their structure
- **ActiveQuestionnaire**: Handles active questionnaire sessions and responses
- **User**: Manages user data and authentication information
- **TrackedRefreshToken**: Handles JWT refresh token lifecycle and security

### Transaction Methods

- **BeginTransactionAsync()**: Starts a new database transaction
- **CommitAsync()**: Commits the current transaction and makes changes permanent
- **RollbackAsync()**: Rolls back the current transaction and discards changes
- **SaveChangesAsync()**: Saves all tracked changes to the database

## How to Use the Unit of Work

### 1. **Dependency Injection**

The Unit of Work is registered in the dependency injection container and can be injected into your services:

```csharp
public class QuestionnaireTemplateService(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    
    // Service methods...
}
```

### 2. **Basic Operations**

For simple operations that don't require explicit transaction management:

```csharp
public async Task<QuestionnaireTemplate> AddTemplate(QuestionnaireTemplateAdd request)
{
    // Perform the operation
    QuestionnaireTemplate template = await _unitOfWork.QuestionnaireTemplate.AddAsync(request);
    
    // Save changes to the database
    await _unitOfWork.SaveChangesAsync();
    
    return template;
}
```

### 3. **Transaction Management**

For complex operations that require explicit transaction control:

```csharp
public async Task<Result> ComplexOperation(ComplexRequest request)
{
    await _unitOfWork.BeginTransactionAsync();
    try
    {
        // Perform multiple related operations
        var step1 = await _unitOfWork.Repository1.DoSomething(request.Data1);
        var step2 = await _unitOfWork.Repository2.DoSomethingElse(request.Data2);
        var step3 = await _unitOfWork.Repository3.FinalizeOperation(step1, step2);
        
        // Save all changes
        await _unitOfWork.SaveChangesAsync();
        
        // Commit the transaction
        await _unitOfWork.CommitAsync();
        
        return step3;
    }
    catch (Exception)
    {
        // Rollback on any error
        await _unitOfWork.RollbackAsync();
        throw;
    }
}
```

### 4. **Working with Multiple Repositories**

The Unit of Work allows you to coordinate operations across multiple repositories:

```csharp
public async Task<ActiveQuestionnaire> ActivateQuestionnaire(ActivateQuestionnaireRequest request)
{
    // Check and add student if needed
    if (!_unitOfWork.User.UserExists(request.StudentId))
    {
        var student = new Student { Id = request.StudentId };
        await _unitOfWork.User.AddStudentAsync(student);
    }
    
    // Check and add teacher if needed
    if (!_unitOfWork.User.UserExists(request.TeacherId))
    {
        var teacher = new Teacher { Id = request.TeacherId };
        await _unitOfWork.User.AddTeacherAsync(teacher);
    }
    
    // Create the active questionnaire
    ActiveQuestionnaire activeQuestionnaire = await _unitOfWork.ActiveQuestionnaire
        .ActivateQuestionnaireAsync(request.TemplateId, request.StudentId, request.TeacherId);
    
    // Save all changes atomically
    await _unitOfWork.SaveChangesAsync();
    
    return activeQuestionnaire;
}
```

## Best Practices

### 1. **Always Save Changes**

Remember to call `SaveChangesAsync()` to persist your changes to the database:

```csharp
// ✅ Good - Changes are saved
await _unitOfWork.Repository.AddAsync(entity);
await _unitOfWork.SaveChangesAsync();

// ❌ Bad - Changes are not persisted
await _unitOfWork.Repository.AddAsync(entity);
// Missing SaveChangesAsync() call
```

### 2. **Use Transactions for Related Operations**

When performing multiple related operations, wrap them in a transaction:

```csharp
// ✅ Good - Multiple operations in a transaction
await _unitOfWork.BeginTransactionAsync();
try 
{
    await _unitOfWork.Repository1.DoOperation1();
    await _unitOfWork.Repository2.DoOperation2();
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitAsync();
}
catch 
{
    await _unitOfWork.RollbackAsync();
    throw;
}
```

### 3. **Handle Exceptions Properly**

Always include proper exception handling with rollback:

```csharp
await _unitOfWork.BeginTransactionAsync();
try 
{
    // Operations that might fail
    await PerformOperations();
    await _unitOfWork.SaveChangesAsync();
    await _unitOfWork.CommitAsync();
}
catch (SpecificException ex)
{
    await _unitOfWork.RollbackAsync();
    // Handle specific exception
    throw new BusinessLogicException("Operation failed", ex);
}
catch (Exception)
{
    await _unitOfWork.RollbackAsync();
    throw; // Re-throw unexpected exceptions
}
```

### 4. **Repository Access Pattern**

Use the repository properties directly from the Unit of Work:

```csharp
// ✅ Good - Direct repository access
var templates = await _unitOfWork.QuestionnaireTemplate.GetAllAsync();
var activeQuestionnaires = await _unitOfWork.ActiveQuestionnaire.GetActiveAsync();

// ❌ Avoid - Don't inject repositories separately when using Unit of Work
```

### 5. **Async/Await Pattern**

Always use async/await for database operations:

```csharp
// ✅ Good - Async operations
public async Task<Result> MyServiceMethod()
{
    var data = await _unitOfWork.Repository.GetDataAsync();
    await _unitOfWork.SaveChangesAsync();
    return data;
}

// ❌ Bad - Blocking operations
public Result MyServiceMethod()
{
    var data = _unitOfWork.Repository.GetDataAsync().Result; // Don't do this
    _unitOfWork.SaveChangesAsync().Wait(); // Don't do this
    return data;
}
```

## Common Patterns in the Codebase

### 1. **Simple CRUD Operations**

```csharp
// Create
public async Task<QuestionnaireTemplate> AddTemplate(QuestionnaireTemplateAdd request)
{
    QuestionnaireTemplate template = await _unitOfWork.QuestionnaireTemplate.AddAsync(request);
    await _unitOfWork.SaveChangesAsync();
    return template;
}

// Read
public async Task<QuestionnaireTemplate> GetTemplateById(Guid id)
{
    return await _unitOfWork.QuestionnaireTemplate.GetById(id);
}

// Update
public async Task<QuestionnaireTemplate> UpdateTemplate(Guid id, QuestionnaireTemplateUpdate updateRequest)
{
    QuestionnaireTemplate updatedTemplate = await _unitOfWork.QuestionnaireTemplate.Update(id, updateRequest);
    await _unitOfWork.SaveChangesAsync();
    return updatedTemplate;
}

// Delete
public async Task DeleteTemplate(Guid id)
{
    await _unitOfWork.QuestionnaireTemplate.DeleteAsync(id);
    await _unitOfWork.SaveChangesAsync();
}
```

### 2. **Pagination Queries**

```csharp
public async Task<PaginationResult> GetActiveQuestionnairesForStudent(PaginationRequest request, Guid userId)
{
    DateTime? cursorActivatedAt = null;
    Guid? cursorId = null;

    if (!string.IsNullOrEmpty(request.QueryCursor))
    {
        cursorActivatedAt = DateTime.Parse(request.QueryCursor.Split('_')[0]);
        cursorId = Guid.Parse(request.QueryCursor.Split('_')[1]);
    }

    (List<ActiveQuestionnaireBase> activeQuestionnaireBases, int totalCount) = 
        await _unitOfWork.ActiveQuestionnaire.PaginationQueryWithKeyset(
            request.PageSize,
            request.Order,
            cursorId,
            cursorActivatedAt,
            request.Title,
            student: request.Teacher,
            idQuery: request.ActiveQuestionnaireId,
            userId: userId,
            isStudentViewPermissions: true
        );

    // Process results...
    return new PaginationResult
    {
        Data = processedResults,
        QueryCursor = nextCursor,
        TotalCount = totalCount
    };
}
```

### 3. **Complex Business Operations**

```csharp
public async Task<SubmissionResult> SubmitAnswers(SubmitAnswersRequest request, Guid userId)
{
    // Validation
    if (await _unitOfWork.ActiveQuestionnaire.HasUserSubmittedAnswer(userId, request.ActiveQuestionnaireId))
    {
        throw new InvalidOperationException("User has already submitted answers for this questionnaire.");
    }

    // Begin transaction for complex operation
    await _unitOfWork.BeginTransactionAsync();
    try
    {
        // Multiple repository operations
        await _unitOfWork.ActiveQuestionnaire.SubmitAnswersAsync(request.ActiveQuestionnaireId, request.Answers, userId);
        await _unitOfWork.User.UpdateLastActivityAsync(userId);
        
        // Save all changes
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
        
        return new SubmissionResult { Success = true };
    }
    catch (Exception)
    {
        await _unitOfWork.RollbackAsync();
        throw;
    }
}
```

## Troubleshooting

### Common Issues

1. **Changes Not Persisted**
   - **Problem**: Entity changes are not saved to the database
   - **Solution**: Ensure you call `await _unitOfWork.SaveChangesAsync()`

2. **Transaction Deadlocks**
   - **Problem**: Multiple transactions waiting for each other
   - **Solution**: Keep transactions short and always use consistent ordering of operations

3. **Memory Leaks**
   - **Problem**: Unit of Work instances not properly disposed
   - **Solution**: The DI container handles disposal automatically in most cases

4. **Concurrency Conflicts**
   - **Problem**: Multiple users modifying the same data simultaneously
   - **Solution**: Handle `DbUpdateConcurrencyException` and implement appropriate retry logic

### Debugging Tips

1. **Enable SQL Logging**: Add logging to see the actual SQL commands being executed
2. **Check Transaction State**: Verify that transactions are properly committed or rolled back
3. **Monitor Database Connections**: Ensure connections are not being leaked

## Summary

The Unit of Work pattern in the NextQuestionnaire Backend provides:

- **Consistency**: All related operations succeed or fail together
- **Performance**: Reduced database round trips through change batching
- **Maintainability**: Centralized transaction and repository management
- **Testability**: Easy mocking and testing of data access operations

By following the patterns and best practices outlined in this guide, you can effectively use the Unit of Work to build robust, consistent, and performant data access operations in your services.
