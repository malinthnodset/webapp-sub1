using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

public class Quiz
{
	public int Id { get; set; }

	[Required(ErrorMessage = "The quiz must have a name.")]
	[StringLength(200)]
	public string Title { get; set; } = string.Empty;

	[StringLength(2000)]
	public string? Description { get; set; }

	// connecting to course (FK)
	[Required(ErrorMessage = "The quiz must belong to an existing course.")]
	public int CourseId { get; set; }
	public virtual Course Course { get; set; } = null!;

	// creator of the quiz (FK)
	public int CreatedByUserId { get; set; }
	public virtual User CreatedByUser { get; set; } = null!;

	// potentially add time stamps for creation/update time
	// public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	// public DateTime? UpdatedAt { get; set; }

	// connect to questions and all taken attempts
	public virtual ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
	public virtual ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
}
