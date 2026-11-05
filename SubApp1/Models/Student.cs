namespace SubApp1.Models;

public class Student : User
{
	// Connect to the quizzes they have created and taken
	public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
	public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();

	// TODO: (optional) add a variable for faculty so a student only gets suggested courses belonging to their faculty
	// or only get access to courses they have registered for exam for
}
