using System;
public class Attempt : BaseEntity
{
    public Guid TestId { get; set; }
    public Test Test { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int CorrectAnswers { get; set; }
    public int TotalQuestions { get; set; }
    public double Score => TotalQuestions > 0 ? (double)CorrectAnswers / TotalQuestions * 100 : 0;
}