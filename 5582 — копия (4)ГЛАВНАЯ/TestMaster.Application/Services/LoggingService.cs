using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class LoggingService : ILoggingService
{
    private readonly AppDbContext _db;
    public LoggingService(AppDbContext db) => _db = db;
    
    public async Task LogAsync(string message)
    {
        _db.LogEntries.Add(new LogEntry { Message = message });
        await _db.SaveChangesAsync();
    }

    public Task<List<LogEntry>> GetLogsAsync()
    {
        return _db.LogEntries.OrderByDescending(l => l.Timestamp).ToListAsync();
    }
}