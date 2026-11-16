using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

// For text answers
public class QuizQuestion
{
	public int Id { get; set; }

	// Connect question to the quiz
	public int QuizId { get; set; }
	public Quiz Quiz { get; set; } = null!;

	[Required(ErrorMessage = "Please add a question.")]
	[StringLength(250)]
	public string Prompt { get; set; } = string.Empty;

	[Required(ErrorMessage = "Write the correct answer.")]
	[StringLength(250)]
	public string CorrectAnswer { get; set; } = string.Empty;

	[Range(0, 10)]
	public decimal Points { get; set; } = 1;

	public int Order { get; set; } // Order of the questions in the quiz

	// Connect users answers
	public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
}
