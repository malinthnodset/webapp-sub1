using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

// For text answers
public class QuizQuestion
{
	public int Id { get; set; }

	public int QuizId { get; set; }
	public Quiz Quiz { get; set; } = null!;

	[Required, StringLength(250)]
	public string Prompt { get; set; } = string.Empty;

	[Required, StringLength(250)]
	public string CorrectAnswer { get; set; } = string.Empty;

	[Range(0.01, 10)]
	public decimal Points { get; set; } = 1;

	public int Order { get; set; }

	public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}
