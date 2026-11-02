using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes

namespace SubApp1.Models; // [LEARN] Domain entities namespace

// [LEARN] What a user has responded on a given question
public class QuizAnswer // [LEARN] Quiz answer entity
{
    public int Id { get; set; } // [LEARN] Primary key

    public int QuizAttemptId { get; set; } // [LEARN] FK to attempt
    public QuizAttempt QuizAttempt { get; set; } = null!; // [LEARN] Navigation

    public int QuizQuestionId { get; set; } // [LEARN] FK to question
    public QuizQuestion QuizQuestion { get; set; } = null!; // [LEARN] Navigation

    [StringLength(2000)] // [LEARN] Validation
    public string? SubmittedAnswer { get; set; } // [LEARN] User's answer

    public bool? IsCorrect { get; set; } // [LEARN] can be null - not yet checked

    [Range(0, 1000)] // [LEARN] Validation
    public decimal PointsAwarded { get; set; } // [LEARN] Points earned
}
