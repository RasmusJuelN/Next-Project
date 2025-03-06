using Database.DTO.ApplicationLog;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

public class ApplicationLogRepository(Context context, ILoggerFactory loggerFactory) : IApplicationLogRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<ApplicationLogsModel> _genericRepository = new(context, loggerFactory);
    public async Task AddAsync(ApplicationLog applicationLog)
    {
        await _genericRepository.AddAsync(applicationLog.ToModel());
    }

    public async Task AddRangeAsync(List<ApplicationLog> applicationLogs)
    {
        await _genericRepository.AddRangeAsync([.. applicationLogs.Select(l => l.ToModel())]);
    }

    public async Task<List<string>> GetLogCategoriesAsync()
    {
        return await _context.ApplicationLogs.Select(q => q.Category).Distinct().ToListAsync();
    }
}
