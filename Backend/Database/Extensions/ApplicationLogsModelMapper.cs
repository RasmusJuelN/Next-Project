using Database.DTO.ApplicationLog;
using Database.Models;

namespace Database.Extensions;

public static class ApplicationLogsModelMapper
{
    public static ApplicationLogsModel ToModel(this ApplicationLog applicationLog)
    {
        return new()
        {
            Message = applicationLog.Message,
            LogLevel = applicationLog.LogLevel,
            Timestamp = applicationLog.Timestamp,
            EventId = applicationLog.EventId,
            Category = applicationLog.Category,
            Exception = applicationLog.Exception
        };
    }
}
