using System.Collections.Generic;

public class Test : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}