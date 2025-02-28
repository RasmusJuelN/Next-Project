using Database.Interfaces;

namespace API.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IQuestionnaireTemplateRepository QuestionnaireTemplate { get; }
    IActiveQuestionnaireRepository ActiveQuestionnaire { get; }
    IUserRepository User { get; }
    ITrackedRefreshTokenRepository TrackedRefreshToken { get; }
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}
