using System;
public class TestAssignment : BaseEntity
{
    public Guid TestId { get; set; }
    public Test Test { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}