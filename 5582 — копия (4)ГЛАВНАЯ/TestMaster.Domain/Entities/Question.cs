using System.Collections.Generic;
public class Question : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public ICollection<AnswerOption> Options { get; set; } = new List<AnswerOption>();
}
