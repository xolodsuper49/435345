using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
public class QuestionService : IQuestionService
{
    private readonly AppDbContext _db;
    public QuestionService(AppDbContext db) => _db = db;

    public Task<List<Question>> GetAllQuestionsAsync() => _db.Questions.Include(q => q.Options).ToListAsync();

    public async Task CreateQuestionAsync(Guid testId, string text, List<(string Text, bool IsCorrect)> options)
{
    var question = new Question { Text = text };
    
    var test = await _db.Tests.FindAsync(testId);
    if (test == null) throw new Exception("Тест не знайдено");
    
    test.Questions.Add(question);

    foreach (var opt in options)
    {
        _db.AnswerOptions.Add(new AnswerOption { Question = question, Text = opt.Text, IsCorrect = opt.IsCorrect });
    }
    await _db.SaveChangesAsync();
}
    
    public async Task UpdateQuestionAsync(Guid questionId, string text, List<(string Text, bool IsCorrect)> options)
    {
        var question = await _db.Questions.Include(q => q.Options).FirstAsync(q => q.Id == questionId);
        question.Text = text;
        _db.AnswerOptions.RemoveRange(question.Options);
        
        foreach (var opt in options)
        {
            _db.AnswerOptions.Add(new AnswerOption { QuestionId = questionId, Text = opt.Text, IsCorrect = opt.IsCorrect });
        }
        await _db.SaveChangesAsync();
    }

    public async Task DeleteQuestionAsync(Guid questionId)
    {
        var question = await _db.Questions.FindAsync(questionId);
        if(question != null)
        {
            _db.Questions.Remove(question);
            await _db.SaveChangesAsync();
        }
    }
}
