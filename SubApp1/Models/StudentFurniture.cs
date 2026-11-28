namespace SubApp1.Models;

// Junction table: which student owns which furniture (N:M)
public class StudentFurniture
{
    public int Id { get; set; }

    // Same int id as QuizAttempt.UserId uses
    public int UserId { get; set; }

    public int FurnitureId { get; set; }

    // nav property
    public virtual Furniture Furniture { get; set; } = null!;

    public DateTime UnlockedAt { get; set; }
}