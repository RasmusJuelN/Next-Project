using Database.DTO.ApplicationLog;

namespace Database.Interfaces;

public interface IApplicationLogRepository
{
    Task AddAsync(ApplicationLog applicationLog);
    Task AddRangeAsync(List<ApplicationLog> applicationLogs);
    Task<List<string>> GetLogCategoriesAsync();
}
