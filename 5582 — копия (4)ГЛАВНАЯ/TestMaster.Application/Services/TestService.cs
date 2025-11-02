using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class TestService : ITestService
{
    private readonly AppDbContext _db;
    public TestService(AppDbContext db) => _db = db;

    public Task<List<Test>> GetAllTestsAsync() => _db.Tests.Include(t => t.Questions).ToListAsync();

    public async Task<Test> CreateTestAsync(string title)
    {
        var test = new Test { Title = title };
        _db.Tests.Add(test);
        await _db.SaveChangesAsync();
        return test;
    }

    public async Task DeleteTestAsync(Guid testId)
    {
        var test = await _db.Tests.FindAsync(testId);
        if (test != null)
        {
            _db.Tests.Remove(test);
            await _db.SaveChangesAsync();
        }
    }
    // ... в кінці класу
    public async Task AssignTestAsync(Guid testId, Guid userId)
    {
        // Перевірка, чи таке призначення вже існує, щоб уникнути дублікатів
        var existing = await _db.TestAssignments
            .AnyAsync(a => a.TestId == testId && a.UserId == userId);

        if (!existing)
        {
            var assignment = new TestAssignment { TestId = testId, UserId = userId };
            _db.TestAssignments.Add(assignment);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<Test>> GetAssignedTestsForStudentAsync(Guid studentId)
    {
        return await _db.TestAssignments
            .Where(a => a.UserId == studentId)
            .Select(a => a.Test)
            .ToListAsync();
    }
    public async Task UpdateTestTitleAsync(Guid testId, string newTitle)
{
    var test = await _db.Tests.FindAsync(testId);
    if (test != null)
    {
        test.Title = newTitle;
        await _db.SaveChangesAsync();
    }
}
}