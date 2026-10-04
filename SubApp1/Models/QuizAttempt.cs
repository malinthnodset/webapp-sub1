namespace SubApp1.Models;

// The complete run through of a quiz, consisting of quiz answers
public class QuizAttempt
{
	public int Id { get; set; }

	public int QuizId { get; set; }
	public Quiz Quiz { get; set; } = null!;

	public int StudentId { get; set; }
	public Student Student { get; set; } = null!;

	public DateTime StartedAt { get; set; } = DateTime.UtcNow;
	public DateTime? CompletedAt { get; set; }

	public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
	public QuizResult? Result { get; set; }
}