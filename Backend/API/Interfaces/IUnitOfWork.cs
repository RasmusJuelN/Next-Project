using Database.Interfaces;

namespace API.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IQuestionnaireTemplateRepository QuestionnaireTemplate { get; }
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task<int> SaveChangesAsync();
}
