using System.ComponentModel.DataAnnotations; // ?

namespace SubApp1.Models;

// What a user has responded on a given question
public class QuizAnswer
{
    public int Id { get; set; }

    public int QuizAttemptId { get; set; }
    public QuizAttempt QuizAttempt { get; set; } = null!;

    public int QuizQuestionId { get; set; }
    public QuizQuestion QuizQuestion { get; set; } = null!;

    [StringLength(2000)]
    public string? SubmittedAnswer { get; set; }

    public bool? IsCorrect { get; set; } // can be null - not yet checked

    [Range(0, 1000)]
    public decimal PointsAwarded { get; set; }
}