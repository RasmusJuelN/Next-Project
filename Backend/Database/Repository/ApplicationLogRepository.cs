using Database.DTO.ApplicationLog;
using Database.Extensions;
using Database.Interfaces;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Database.Repository;

/// <summary>
/// Implements repository operations for application log management.
/// Provides functionality for persisting application logs and retrieving log categorization information for monitoring and debugging.
/// </summary>
/// <remarks>
/// This repository handles the storage and organization of application logging data, supporting both individual
/// and bulk log operations for efficient performance. Enables categorization and filtering of logs for
/// monitoring dashboards and debugging workflows.
/// </remarks>
/// <param name="context">The database context for data access operations.</param>
/// <param name="loggerFactory">Factory for creating loggers for diagnostic and monitoring purposes.</param>
public class ApplicationLogRepository(Context context, ILoggerFactory loggerFactory) : IApplicationLogRepository
{
    private readonly Context _context = context;
    private readonly GenericRepository<ApplicationLogsModel> _genericRepository = new(context, loggerFactory);
    
    /// <summary>
    /// Adds a single application log entry to the database.
    /// </summary>
    /// <param name="applicationLog">The ApplicationLog DTO containing log information to persist.</param>
    /// <remarks>
    /// Converts the DTO to the appropriate model format before persistence.
    /// For bulk operations, prefer AddRangeAsync for better performance.
    /// </remarks>
    public async Task AddAsync(ApplicationLog applicationLog)
    {
        await _genericRepository.AddAsync(applicationLog.ToModel());
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Adds multiple application log entries to the database in a batch operation.
    /// </summary>
    /// <param name="applicationLogs">A list of ApplicationLog DTOs to persist.</param>
    /// <remarks>
    /// Provides optimized bulk insertion by converting all DTOs to models and using batch operations.
    /// More efficient than multiple individual AddAsync calls for large log batches.
    /// </remarks>
    public async Task AddRangeAsync(List<ApplicationLog> applicationLogs)
    {
        await _genericRepository.AddRangeAsync([.. applicationLogs.Select(l => l.ToModel())]);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all distinct log categories currently stored in the database.
    /// </summary>
    /// <returns>A list of strings representing unique log categories.</returns>
    /// <remarks>
    /// Used for populating filter dropdowns and organizing logs by functional area.
    /// Categories help identify which application components or subsystems generated specific log entries.
    /// </remarks>
    public async Task<List<string>> GetLogCategoriesAsync()
    {
        return await _context.ApplicationLogs.Select(q => q.Category).Distinct().ToListAsync();
    }


    /// <inheritdoc/>
    public async Task<List<int>> GetLogEventIDsAsync()
    {
        return await _context.ApplicationLogs.Select(log => log.EventId).Distinct().ToListAsync();
    }
}
