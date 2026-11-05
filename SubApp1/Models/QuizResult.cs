using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

// Content of a result page after a quiz has been completed (belongs to an attempt)
public class QuizResult
{
	public int Id { get; set; }

	public int QuizAttemptId { get; set; }
	public QuizAttempt QuizAttempt { get; set; } = null!;

	[Range(0, 100000)]
	public decimal PointsEarned { get; set; }

	[Range(0, 100000)]
	public decimal MaximumPoints { get; set; }

	public int CorrectAnswers { get; set; }
	public int TotalQuestions { get; set; }

	// Calculates the % score of a quiz attempt (whole quiz completed)
    public decimal Percentage => MaximumPoints <= 0
		? 0
		: Math.Round(PointsEarned / MaximumPoints * 100, 2);
}
