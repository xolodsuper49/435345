using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ITestService
{
    Task<List<Test>> GetAllTestsAsync();
    Task<Test> CreateTestAsync(string title);
    Task DeleteTestAsync(Guid testId);
    Task UpdateTestTitleAsync(Guid testId, string newTitle);
    Task AssignTestAsync(Guid testId, Guid userId);
    Task<List<Test>> GetAssignedTestsForStudentAsync(Guid studentId);
}