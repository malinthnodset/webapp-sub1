namespace SubApp1.Models; // [LEARN] Domain entities namespace

// [LEARN] The complete run through of a quiz, consisting of quiz answers
public class QuizAttempt // [LEARN] Quiz attempt entity
{
public int Id { get; set; } // [LEARN] Primary key

public int QuizId { get; set; } // [LEARN] FK to quiz
public Quiz Quiz { get; set; } = null!; // [LEARN] Navigation

public int StudentId { get; set; } // [LEARN] FK to student
public Student Student { get; set; } = null!; // [LEARN] Navigation

public DateTime StartedAt { get; set; } = DateTime.UtcNow; // [LEARN] Start time
public DateTime? CompletedAt { get; set; } // [LEARN] Completion time

public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>(); // [LEARN] Navigation: answers
public QuizResult? Result { get; set; } // [LEARN] Navigation: result
}
