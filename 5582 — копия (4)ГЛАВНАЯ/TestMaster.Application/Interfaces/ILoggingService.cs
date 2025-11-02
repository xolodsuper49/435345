using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILoggingService
{
    Task LogAsync(string message);
    Task<List<LogEntry>> GetLogsAsync();
}