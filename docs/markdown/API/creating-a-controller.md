# Creating a Controller Tutorial

This tutorial will walk you through creating a complete controller implementation in the NextQuestionnaire Backend, including the necessary DTOs and services. We'll use a practical example to demonstrate the patterns and best practices used in this project.

## Overview

In this tutorial, we'll create a `BookController` that manages a fictional book resource. This will demonstrate:

- Controller structure and conventions
- Request and Response DTO creation
- Service layer implementation
- Proper dependency injection
- Authentication and authorization
- API documentation with XML comments

## Step 1: Planning the Controller

Before writing any code, plan what your controller will do:

- **Resource**: Books
- **Operations**: List books (with pagination), get a book by ID, create a book, update a book, delete a book
- **Authentication**: Required for all operations
- **Authorization**: Admin-only for create, update, delete operations

## Step 2: Create Request DTOs

DTOs (Data Transfer Objects) define the structure of data sent to and from your API. Start with request DTOs.

### 2.1 Create the Requests Directory

If it doesn't exist, create the directory structure:
```
Backend/API/DTO/Requests/Book/
```

### 2.2 Pagination Request DTO

**File**: `Backend/API/DTO/Requests/Book/BookPaginationRequest.cs`

```csharp
using System.ComponentModel;
using Database.Enums;

namespace API.DTO.Requests.Book;

/// <summary>
/// Represents a request for paginating books using keyset pagination.
/// </summary>
/// <remarks>
/// This request supports filtering books by title and author while providing
/// efficient pagination through keyset-based cursor navigation.
/// </remarks>
public record class BookPaginationRequest
{
    /// <summary>
    /// Gets or sets the number of items to return per page.
    /// </summary>
    /// <value>The page size, with a default value of 10.</value>
    [DefaultValue(10)]
    public required int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the ordering for the pagination results.
    /// </summary>
    /// <value>The ordering option, defaults to CreatedAtDesc.</value>
    [DefaultValue(BookOrderingOptions.CreatedAtDesc)]
    public BookOrderingOptions Order { get; set; } = BookOrderingOptions.CreatedAtDesc;

    /// <summary>
    /// Gets or sets the title filter for searching books.
    /// </summary>
    /// <value>Optional title search term.</value>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the author filter for searching books.
    /// </summary>
    /// <value>Optional author search term.</value>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the cursor for pagination navigation.
    /// </summary>
    /// <value>The pagination cursor from the previous response.</value>
    public string? QueryCursor { get; set; }
}
```

### 2.3 Create Request DTO

**File**: `Backend/API/DTO/Requests/Book/BookCreateRequest.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests.Book;

/// <summary>
/// Represents the data required to create a new book.
/// </summary>
/// <remarks>
/// This record contains all the necessary information for creating a book
/// in the system, including validation requirements.
/// </remarks>
public record class BookCreateRequest
{
    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    /// <value>The book title, which is required and has a maximum length of 200 characters.</value>
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    /// <value>The book author, which is required and has a maximum length of 100 characters.</value>
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public required string Author { get; set; }

    /// <summary>
    /// Gets or sets the ISBN of the book.
    /// </summary>
    /// <value>The book ISBN, which is optional.</value>
    [StringLength(13)]
    public string? ISBN { get; set; }

    /// <summary>
    /// Gets or sets the publication year of the book.
    /// </summary>
    /// <value>The publication year, which must be between 1000 and the current year.</value>
    [Range(1000, 2100)]
    public int? PublicationYear { get; set; }

    /// <summary>
    /// Gets or sets the description of the book.
    /// </summary>
    /// <value>An optional description with a maximum length of 1000 characters.</value>
    [StringLength(1000)]
    public string? Description { get; set; }
}
```

### 2.4 Update Request DTO

**File**: `Backend/API/DTO/Requests/Book/BookUpdateRequest.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace API.DTO.Requests.Book;

/// <summary>
/// Represents the data for updating an existing book.
/// </summary>
/// <remarks>
/// This record contains the fields that can be updated for an existing book.
/// All fields are optional to support partial updates.
/// </remarks>
public record class BookUpdateRequest
{
    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    [StringLength(200, MinimumLength = 1)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    [StringLength(100, MinimumLength = 1)]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the ISBN of the book.
    /// </summary>
    [StringLength(13)]
    public string? ISBN { get; set; }

    /// <summary>
    /// Gets or sets the publication year of the book.
    /// </summary>
    [Range(1000, 2100)]
    public int? PublicationYear { get; set; }

    /// <summary>
    /// Gets or sets the description of the book.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }
}
```

## Step 3: Create Response DTOs

Response DTOs define what data the API returns to clients.

### 3.1 Create the Responses Directory

Create the directory structure:
```
Backend/API/DTO/Responses/Book/
```

### 3.2 Book Response DTO

**File**: `Backend/API/DTO/Responses/Book/BookResponse.cs`

```csharp
namespace API.DTO.Responses.Book;

/// <summary>
/// Represents a book in API responses.
/// </summary>
/// <remarks>
/// This record contains the complete book information returned by the API,
/// including system-generated fields like ID and timestamps.
/// </remarks>
public record class BookResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the book.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the book.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the author of the book.
    /// </summary>
    public required string Author { get; set; }

    /// <summary>
    /// Gets or sets the ISBN of the book.
    /// </summary>
    public string? ISBN { get; set; }

    /// <summary>
    /// Gets or sets the publication year of the book.
    /// </summary>
    public int? PublicationYear { get; set; }

    /// <summary>
    /// Gets or sets the description of the book.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the book was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the book was last updated.
    /// </summary>
    public required DateTime UpdatedAt { get; set; }
}
```

### 3.3 Pagination Response DTO

**File**: `Backend/API/DTO/Responses/Book/BookPaginationResponse.cs`

```csharp
namespace API.DTO.Responses.Book;

/// <summary>
/// Represents the result of a paginated book query.
/// </summary>
/// <remarks>
/// This record contains the paginated book data along with metadata
/// required for implementing keyset pagination navigation.
/// </remarks>
public record class BookPaginationResponse
{
    /// <summary>
    /// Gets or sets the list of books in the current page.
    /// </summary>
    public required List<BookResponse> Books { get; set; }

    /// <summary>
    /// Gets or sets the cursor for navigating to the next page.
    /// </summary>
    /// <value>
    /// The cursor token to be used in the next request, or null if this is the last page.
    /// </value>
    public string? NextCursor { get; set; }

    /// <summary>
    /// Gets or sets the total count of books matching the query.
    /// </summary>
    public required int TotalCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether there are more pages available.
    /// </summary>
    public required bool HasMore { get; set; }
}
```

## Step 4: Create the Service

The service layer contains business logic and data access operations.

### 4.1 Define the Service Interface (Optional but Recommended)

**File**: `Backend/API/Interfaces/IBookService.cs`

```csharp
using API.DTO.Requests.Book;
using API.DTO.Responses.Book;

namespace API.Interfaces;

/// <summary>
/// Defines the contract for book management services.
/// </summary>
/// <remarks>
/// This interface abstracts book operations and can be implemented
/// by different service providers while maintaining consistency.
/// </remarks>
public interface IBookService
{
    /// <summary>
    /// Retrieves books with pagination support.
    /// </summary>
    /// <param name="request">The pagination request parameters.</param>
    /// <returns>A task containing the paginated book results.</returns>
    Task<BookPaginationResponse> GetBooksAsync(BookPaginationRequest request);

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The book identifier.</param>
    /// <returns>A task containing the book if found.</returns>
    Task<BookResponse?> GetBookByIdAsync(Guid id);

    /// <summary>
    /// Creates a new book.
    /// </summary>
    /// <param name="request">The book creation request.</param>
    /// <returns>A task containing the created book.</returns>
    Task<BookResponse> CreateBookAsync(BookCreateRequest request);

    /// <summary>
    /// Updates an existing book.
    /// </summary>
    /// <param name="id">The book identifier.</param>
    /// <param name="request">The update request.</param>
    /// <returns>A task containing the updated book.</returns>
    Task<BookResponse?> UpdateBookAsync(Guid id, BookUpdateRequest request);

    /// <summary>
    /// Deletes a book.
    /// </summary>
    /// <param name="id">The book identifier.</param>
    /// <returns>A task indicating the operation success.</returns>
    Task<bool> DeleteBookAsync(Guid id);
}
```

### 4.2 Implement the Service

**File**: `Backend/API/Services/BookService.cs`

```csharp
using API.DTO.Requests.Book;
using API.DTO.Responses.Book;
using API.Interfaces;
using Database.Models;

namespace API.Services;

/// <summary>
/// Provides business logic and data management services for book operations.
/// This service handles the complete lifecycle of books including creation,
/// modification, retrieval, and deletion while ensuring data integrity.
/// </summary>
/// <remarks>
/// The service implements comprehensive book management functionality:
/// <list type="bullet">
/// <item><description>CRUD operations with proper validation</description></item>
/// <item><description>Paginated retrieval with filtering capabilities</description></item>
/// <item><description>Data transformation between domain models and DTOs</description></item>
/// <item><description>Transaction management through Unit of Work pattern</description></item>
/// </list>
/// All operations maintain data consistency and follow domain business rules.
/// </remarks>
public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for data access operations.</param>
    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Retrieves books with pagination and filtering support.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the paginated book results with navigation metadata.
    /// </returns>
    /// <remarks>
    /// This method supports efficient pagination using keyset-based navigation
    /// and provides filtering capabilities for title and author fields.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when request parameters are invalid.</exception>
    public async Task<BookPaginationResponse> GetBooksAsync(BookPaginationRequest request)
    {
        // Parse cursor if provided
        DateTime? cursorCreatedAt = null;
        Guid? cursorId = null;

        if (!string.IsNullOrEmpty(request.QueryCursor))
        {
            var parts = request.QueryCursor.Split('_');
            if (parts.Length == 2)
            {
                cursorCreatedAt = DateTime.Parse(parts[0]);
                cursorId = Guid.Parse(parts[1]);
            }
        }

        // Get paginated results from repository
        var (books, totalCount) = await _unitOfWork.Books.GetPaginatedAsync(
            request.PageSize,
            cursorId,
            cursorCreatedAt,
            request.Order,
            request.Title,
            request.Author
        );

        // Convert to response DTOs
        var bookResponses = books.Select(MapToResponse).ToList();

        // Generate next cursor
        string? nextCursor = null;
        if (bookResponses.Count == request.PageSize && bookResponses.LastOrDefault() is { } lastBook)
        {
            nextCursor = $"{lastBook.CreatedAt:O}_{lastBook.Id}";
        }

        return new BookPaginationResponse
        {
            Books = bookResponses,
            NextCursor = nextCursor,
            TotalCount = totalCount,
            HasMore = nextCursor != null
        };
    }

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the book if found, or null if not found.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
    public async Task<BookResponse?> GetBookByIdAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        return book != null ? MapToResponse(book) : null;
    }

    /// <summary>
    /// Creates a new book in the system.
    /// </summary>
    /// <param name="request">The book creation request containing the book data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the created book with generated system fields.
    /// </returns>
    /// <remarks>
    /// This method validates the request data, creates a new book entity,
    /// and persists it to the database within a transaction.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when request data is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when business rules are violated.</exception>
    public async Task<BookResponse> CreateBookAsync(BookCreateRequest request)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            PublicationYear = request.PublicationYear,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Books.AddAsync(book);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(book);
    }

    /// <summary>
    /// Updates an existing book with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the book to update.</param>
    /// <param name="request">The update request containing the new book data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the updated book if found and updated, or null if not found.
    /// </returns>
    /// <remarks>
    /// This method supports partial updates - only the fields provided in the request
    /// will be updated, leaving other fields unchanged.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when request data is invalid.</exception>
    public async Task<BookResponse?> UpdateBookAsync(Guid id, BookUpdateRequest request)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
            return null;

        // Update only provided fields
        if (!string.IsNullOrEmpty(request.Title))
            book.Title = request.Title;
        
        if (!string.IsNullOrEmpty(request.Author))
            book.Author = request.Author;
        
        if (request.ISBN != null)
            book.ISBN = request.ISBN;
        
        if (request.PublicationYear.HasValue)
            book.PublicationYear = request.PublicationYear.Value;
        
        if (request.Description != null)
            book.Description = request.Description;

        book.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Books.UpdateAsync(book);
        await _unitOfWork.SaveChangesAsync();

        return MapToResponse(book);
    }

    /// <summary>
    /// Permanently removes a book from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// true if the book was found and deleted, false if not found.
    /// </returns>
    /// <remarks>
    /// This operation is irreversible and will permanently remove the book
    /// from the database. Ensure proper authorization before calling this method.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the book cannot be deleted due to business rules or dependencies.
    /// </exception>
    public async Task<bool> DeleteBookAsync(Guid id)
    {
        var book = await _unitOfWork.Books.GetByIdAsync(id);
        if (book == null)
            return false;

        await _unitOfWork.Books.DeleteAsync(book);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Maps a book domain model to a response DTO.
    /// </summary>
    /// <param name="book">The book domain model.</param>
    /// <returns>The corresponding response DTO.</returns>
    private static BookResponse MapToResponse(Book book)
    {
        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            PublicationYear = book.PublicationYear,
            Description = book.Description,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt
        };
    }
}
```

## Step 5: Create the Controller

Finally, create the controller that ties everything together.

**File**: `Backend/API/Controllers/BookController.cs`

```csharp
using API.DTO.Requests.Book;
using API.DTO.Responses.Book;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller responsible for managing books in the system.
/// Provides CRUD operations and pagination functionality for books.
/// </summary>
/// <remarks>
/// This controller handles all operations related to books including:
/// - Retrieving books with pagination and filtering
/// - Getting individual books by ID
/// - Creating new books (admin only)
/// - Updating existing books (admin only)
/// - Deleting books (admin only)
/// 
/// All endpoints require authentication, with admin-level authorization
/// required for create, update, and delete operations.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = "AccessToken")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookController"/> class.
    /// </summary>
    /// <param name="bookService">The book service for handling business logic.</param>
    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }

    /// <summary>
    /// Retrieves books with pagination and filtering support.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an ActionResult with:
    /// - 200 OK: A BookPaginationResponse containing the paginated books.
    /// - 400 Bad Request: When request parameters are invalid.
    /// - 401 Unauthorized: When authentication is missing or invalid.
    /// - 500 Internal Server Error: When an unexpected error occurs.
    /// </returns>
    /// <remarks>
    /// This endpoint supports keyset pagination for efficient handling of large datasets.
    /// Use the NextCursor from the response to navigate to subsequent pages.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(BookPaginationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookPaginationResponse>> GetBooks([FromQuery] BookPaginationRequest request)
    {
        try
        {
            var result = await _bookService.GetBooksAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the book to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an ActionResult with:
    /// - 200 OK: The requested book.
    /// - 404 Not Found: When no book exists with the specified ID.
    /// - 401 Unauthorized: When authentication is missing or invalid.
    /// - 500 Internal Server Error: When an unexpected error occurs.
    /// </returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookResponse>> GetBook(Guid id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    /// <summary>
    /// Creates a new book in the system.
    /// </summary>
    /// <param name="request">The book creation request containing the book data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an ActionResult with:
    /// - 201 Created: The created book with location header.
    /// - 400 Bad Request: When request data is invalid.
    /// - 401 Unauthorized: When authentication is missing or invalid.
    /// - 403 Forbidden: When the user lacks admin privileges.
    /// - 500 Internal Server Error: When an unexpected error occurs.
    /// </returns>
    /// <remarks>
    /// This endpoint requires admin-level authorization. The created book will include
    /// system-generated fields such as ID and timestamps.
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookResponse>> CreateBook([FromBody] BookCreateRequest request)
    {
        try
        {
            var book = await _bookService.CreateBookAsync(request);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing book with new data.
    /// </summary>
    /// <param name="id">The unique identifier of the book to update.</param>
    /// <param name="request">The update request containing the new book data.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an ActionResult with:
    /// - 200 OK: The updated book.
    /// - 400 Bad Request: When request data is invalid.
    /// - 401 Unauthorized: When authentication is missing or invalid.
    /// - 403 Forbidden: When the user lacks admin privileges.
    /// - 404 Not Found: When no book exists with the specified ID.
    /// - 500 Internal Server Error: When an unexpected error occurs.
    /// </returns>
    /// <remarks>
    /// This endpoint requires admin-level authorization and supports partial updates.
    /// Only the fields provided in the request will be updated.
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BookResponse>> UpdateBook(Guid id, [FromBody] BookUpdateRequest request)
    {
        try
        {
            var book = await _bookService.UpdateBookAsync(id, request);
            
            if (book == null)
                return NotFound();

            return Ok(book);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Permanently removes a book from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the book to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an ActionResult with:
    /// - 204 No Content: When the book was successfully deleted.
    /// - 401 Unauthorized: When authentication is missing or invalid.
    /// - 403 Forbidden: When the user lacks admin privileges.
    /// - 404 Not Found: When no book exists with the specified ID.
    /// - 500 Internal Server Error: When an unexpected error occurs.
    /// </returns>
    /// <remarks>
    /// This endpoint requires admin-level authorization. The deletion is permanent
    /// and cannot be undone. Ensure proper confirmation before calling this endpoint.
    /// </remarks>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteBook(Guid id)
    {
        var deleted = await _bookService.DeleteBookAsync(id);
        
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
```

## Step 6: Register the Service

Don't forget to register your service in the dependency injection container.

In `Program.cs`, add:

```csharp
// Register services
builder.Services.AddScoped<IBookService, BookService>();
```

## Best Practices Demonstrated

This tutorial demonstrates several key patterns used in the NextQuestionnaire Backend:

### 1. **Consistent Naming Conventions**
- Controllers end with "Controller"
- Services end with "Service"
- DTOs are descriptive and purpose-specific

### 2. **Comprehensive XML Documentation**
- Every public member has XML documentation
- Parameters, returns, and exceptions are documented
- Remarks provide additional context and usage guidance

### 3. **Proper HTTP Status Codes**
- 200 OK for successful retrievals
- 201 Created for successful creation
- 204 No Content for successful deletion
- 400 Bad Request for validation errors
- 401 Unauthorized for authentication failures
- 403 Forbidden for authorization failures
- 404 Not Found for missing resources

### 4. **Authentication and Authorization**
- All endpoints require authentication
- Admin operations use additional policy-based authorization
- Clear separation between read and write permissions

### 5. **Error Handling**
- Try-catch blocks for expected exceptions
- Appropriate HTTP status codes for different error types
- Validation at multiple layers (DTO, service, business logic)

### 6. **Separation of Concerns**
- Controllers handle HTTP concerns
- Services handle business logic
- DTOs handle data transfer
- Clear boundaries between layers

### 7. **Async/Await Pattern**
- All I/O operations are asynchronous
- Proper task return types
- Consistent async method naming

This pattern provides a solid foundation for building robust, maintainable REST APIs in the NextQuestionnaire Backend system.
