using Database.Interfaces;

namespace API.Interfaces;

/// <summary>
/// Defines a unit of work pattern interface that provides access to repositories and manages database transactions.
/// Implements IDisposable to ensure proper resource cleanup.
/// </summary>
/// <remarks>
/// The Unit of Work pattern maintains a list of objects affected by a business transaction and 
/// coordinates writing out changes and resolving concurrency problems.
/// </remarks>
public interface IUnitOfWork : IDisposable
{
    IQuestionnaireTemplateRepository QuestionnaireTemplate { get; }
    IActiveQuestionnaireRepository ActiveQuestionnaire { get; }
    IUserRepository User { get; }
    ITrackedRefreshTokenRepository TrackedRefreshToken { get; }
    IQuestionnaireGroupRepository QuestionnaireGroup { get; }
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}
