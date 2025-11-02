using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class AttemptService : IAttemptService
{
    private readonly AppDbContext _db;
    public AttemptService(AppDbContext db) => _db = db;
    
    public async Task SaveAttemptAsync(Guid testId, Guid userId, int correctAnswers, int totalQuestions)
    {
        var attempt = new Attempt { TestId = testId, UserId = userId, CorrectAnswers = correctAnswers, TotalQuestions = totalQuestions };
        _db.Attempts.Add(attempt);
        await _db.SaveChangesAsync();
    }

    public Task<List<Attempt>> GetAttemptsForStudentAsync(Guid studentId)
    {
        return _db.Attempts.Where(a => a.UserId == studentId).Include(a => a.Test).ToListAsync();
    }
}