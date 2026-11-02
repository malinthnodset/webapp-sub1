using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes

namespace SubApp1.Models; // [LEARN] Domain entities namespace

// [LEARN] As of now the only user group - later implementation will split into teachers and students
public class Student // [LEARN] Student entity
{
public int Id { get; set; } // [LEARN] Primary key

[Required, StringLength(200)] // [LEARN] Validation
public string Name { get; set; } = string.Empty; // [LEARN] Name

[Required, EmailAddress, StringLength(320)] // [LEARN] Validation
public string Email { get; set; } = string.Empty; // [LEARN] Email

    /* TODO: Add a variable for faculty so a student only gets suggested courses 
belonging to their faculty or only get access to courses they have registered for exam for */
public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>(); // [LEARN] Navigation: created quizzes
public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>(); // [LEARN] Navigation: attempts
}
