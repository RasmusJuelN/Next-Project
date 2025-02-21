using API.Interfaces;
using Database;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Services;

public class UnitOfWork(Context context, IQuestionnaireTemplateRepository templateRepository) : IUnitOfWork
{
    private readonly Context _context = context;
    private IDbContextTransaction? _transaction;
    public IQuestionnaireTemplateRepository QuestionnaireTemplate { get; } = templateRepository;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

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
