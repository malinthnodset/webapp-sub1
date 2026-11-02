using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes

namespace SubApp1.Models; // [LEARN] Domain entities namespace

public class Quiz // [LEARN] Quiz entity
{
public int Id { get; set; } // [LEARN] Primary key

[Required, StringLength(200)] // [LEARN] Validation
public string Title { get; set; } = string.Empty; // [LEARN] Quiz title

[StringLength(2000)] // [LEARN] Validation
public string? Description { get; set; } // [LEARN] Optional description

// [LEARN] connecting to course (FK)
public int CourseId { get; set; } // [LEARN] Foreign key to Course
public Course Course { get; set; } = null!; // [LEARN] must belong to a course

    // [LEARN] creator of the quiz (FK)
public int CreatedByStudentId { get; set; } // [LEARN] Foreign key to Student
public Student CreatedByStudent { get; set; } = null!; // [LEARN] Creator

    // [LEARN] potentially add time stamps
// public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
// public DateTime? UpdatedAt { get; set; }

    // [LEARN] connect to questions and all taken attempts (for logged in student)
public ICollection<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>(); // [LEARN] Navigation: questions
public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>(); // [LEARN] Navigation: attempts
}
