using Microsoft.EntityFrameworkCore;
using TestMaster.Infrastructure.Services;
public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<AnswerOption> AnswerOptions => Set<AnswerOption>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<TestAssignment> TestAssignments => Set<TestAssignment>();
    public DbSet<Attempt> Attempts => Set<Attempt>();       // <-- ДОДАЙТЕ
    public DbSet<LogEntry> LogEntries => Set<LogEntry>(); // <-- ДОДАЙТЕ
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public static async System.Threading.Tasks.Task InitializeDatabaseAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        if (await context.Users.AnyAsync()) return;

        var users = new[]
        {
            new User { UserName = "teacher", Role = TestMaster.Domain.Enums.Role.Teacher, PasswordHash = PasswordHelper.HashPassword("teacher") },
            new User { UserName = "student", Role = TestMaster.Domain.Enums.Role.Student, PasswordHash = PasswordHelper.HashPassword("student") }
        };
        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}
