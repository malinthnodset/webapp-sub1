using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

public class QuizQuestion
{
	public int Id { get; set; }

	public int QuizId { get; set; }
	public Quiz Quiz { get; set; } = null!;

	[Required, StringLength(200)]
	public string Prompt { get; set; } = string.Empty;

	[Required, StringLength(2000)]
	public string CorrectAnswer { get; set; } = string.Empty;

	[Range(0.01, 1000)]
	public decimal Points { get; set; } = 1;

	public int Order { get; set; }

	public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}
