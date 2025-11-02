using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public interface IQuestionService
{
    Task<List<Question>> GetAllQuestionsAsync();
    Task CreateQuestionAsync(Guid testId, string text, List<(string Text, bool IsCorrect)> options);
    Task UpdateQuestionAsync(Guid questionId, string text, List<(string Text, bool IsCorrect)> options);
    Task DeleteQuestionAsync(Guid questionId);
}
