using System.ComponentModel.DataAnnotations; // [LEARN] Validation attributes

namespace SubApp1.Models; // [LEARN] Domain entities namespace

// [LEARN] For text answers
public class QuizQuestion // [LEARN] Quiz question entity
{
public int Id { get; set; } // [LEARN] Primary key

public int QuizId { get; set; } // [LEARN] FK to quiz
public Quiz Quiz { get; set; } = null!; // [LEARN] Navigation

[Required, StringLength(250)] // [LEARN] Validation
public string Prompt { get; set; } = string.Empty; // [LEARN] Question text

[Required, StringLength(250)] // [LEARN] Validation
public string CorrectAnswer { get; set; } = string.Empty; // [LEARN] Correct answer

[Range(0.01, 10)] // [LEARN] Validation
public decimal Points { get; set; } = 1; // [LEARN] Points value

public int Order { get; set; } // [LEARN] Display order

public ICollection<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>(); // [LEARN] Navigation: answers
}
