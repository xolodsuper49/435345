using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAttemptService
{
    Task SaveAttemptAsync(Guid testId, Guid userId, int correctAnswers, int totalQuestions);
    Task<List<Attempt>> GetAttemptsForStudentAsync(Guid studentId);
}