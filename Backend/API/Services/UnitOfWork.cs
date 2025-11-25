using API.Interfaces;
using Database;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Services;

/// <summary>
/// Implements the Unit of Work pattern to coordinate database operations across multiple repositories.
/// This service provides transaction management, change tracking, and ensures data consistency
/// by grouping related operations into atomic transactions that either succeed or fail together.
/// </summary>
/// <remarks>
/// The Unit of Work pattern provides several key benefits:
/// <list type="bullet">
/// <item><description>Transaction management: Ensures atomicity across multiple repository operations</description></item>
/// <item><description>Change tracking: Coordinates Entity Framework change detection and saving</description></item>
/// <item><description>Resource management: Proper disposal of database connections and transactions</description></item>
/// <item><description>Repository coordination: Central access point for all data access repositories</description></item>
/// </list>
/// All repository operations should be performed through this unit of work to ensure proper transaction handling.
/// </remarks>
public class UnitOfWork(
    Context context,
    IQuestionnaireTemplateRepository templateRepository,
    IActiveQuestionnaireRepository activeQuestionnaire,
    IUserRepository user,
    ITrackedRefreshTokenRepository trackedRefreshToken,
    IQuestionnaireGroupRepository groupRepository) : IUnitOfWork
{
    private readonly Context _context = context;
    private IDbContextTransaction? _transaction;
    
    /// <summary>
    /// Gets the repository for questionnaire template operations.
    /// </summary>
    /// <value>
    /// The questionnaire template repository instance for managing questionnaire templates and their associated data.
    /// </value>
    public IQuestionnaireTemplateRepository QuestionnaireTemplate { get; } = templateRepository;
    
    /// <summary>
    /// Gets the repository for active questionnaire operations.
    /// </summary>
    /// <value>
    /// The active questionnaire repository instance for managing active questionnaire sessions and responses.
    /// </value>
    public IActiveQuestionnaireRepository ActiveQuestionnaire { get; } = activeQuestionnaire;
    
    /// <summary>
    /// Gets the repository for user operations.
    /// </summary>
    /// <value>
    /// The user repository instance for managing user data and authentication information.
    /// </value>
    public IUserRepository User { get; } = user;
    
    /// <summary>
    /// Gets the repository for refresh token tracking operations.
    /// </summary>
    /// <value>
    /// The tracked refresh token repository instance for managing JWT refresh token lifecycle and security.
    /// </value>
    public ITrackedRefreshTokenRepository TrackedRefreshToken { get; } = trackedRefreshToken;
    public IQuestionnaireGroupRepository QuestionnaireGroup { get; } = groupRepository;

    /// <summary>
    /// Asynchronously saves all pending changes in the current unit of work to the database.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous save operation. The task result contains
    /// the number of state entries written to the database.
    /// </returns>
    /// <remarks>
    /// This method commits all tracked changes across all repositories in a single database operation.
    /// If a transaction is active, changes are staged until the transaction is committed or rolled back.
    /// The operation will fail atomically if any constraint violations or data conflicts occur.
    /// </remarks>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">
    /// Thrown when an error occurs while updating the database.
    /// </exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
    /// Thrown when a concurrency conflict is encountered while saving changes.
    /// </exception>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Begins a new database transaction for coordinating multiple operations.
    /// </summary>
    /// <returns>A task that represents the asynchronous transaction initialization operation.</returns>
    /// <remarks>
    /// This method starts a new database transaction that allows multiple operations to be grouped together.
    /// All subsequent database operations will be part of this transaction until it is committed or rolled back.
    /// Only one transaction can be active per unit of work instance at a time.
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a transaction is already active for this unit of work.
    /// </exception>
    /// <exception cref="System.Data.Common.DbException">
    /// Thrown when the database provider encounters an error while beginning the transaction.
    /// </exception>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
        }
    }

    public async Task CommitAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}
