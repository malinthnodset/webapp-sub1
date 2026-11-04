using System.ComponentModel.DataAnnotations;

namespace SubApp1.Models;

// As of now the only user group - later implementation will split into teachers and students
public class Student
{
	public int Id { get; set; }

	[Required(ErrorMessage = "Name cannot be empty.")]
	[StringLength(200)]
	public string Name { get; set; } = string.Empty;

	[Required(ErrorMessage = "Email address cannot be empty."), EmailAddress, StringLength(320)]
	public string Email { get; set; } = string.Empty;

	// Connect to the quizzes they have created and taken
	public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
	public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();

	// TODO: (optional) add a variable for faculty so a student only gets suggested courses belonging to their faculty
	// or only get access to courses they have registered for exam for

}
