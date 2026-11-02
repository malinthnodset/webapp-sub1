using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes

namespace SubApp1.Models; // [LEARN] Domain entities namespace

// [LEARN] Content of a result page after a quiz has been completed (belongs to an attempt)
public class QuizResult // [LEARN] Quiz result entity
{
public int Id { get; set; } // [LEARN] Primary key

public int QuizAttemptId { get; set; } // [LEARN] FK to attempt
public QuizAttempt QuizAttempt { get; set; } = null!; // [LEARN] Navigation

[Range(0, 100000)] // [LEARN] Validation
public decimal PointsEarned { get; set; } // [LEARN] Points earned

[Range(0, 100000)] // [LEARN] Validation
public decimal MaximumPoints { get; set; } // [LEARN] Max points

public int CorrectAnswers { get; set; } // [LEARN] Correct count
public int TotalQuestions { get; set; } // [LEARN] Total count

// [LEARN] Calculates the % score of a quiz attempt (whole quiz completed)
    public decimal Percentage => MaximumPoints <= 0 // [LEARN] Avoid div by zero
? 0 // [LEARN] 0 if no points
: Math.Round(PointsEarned / MaximumPoints * 100, 2); // [LEARN] Percentage
}
