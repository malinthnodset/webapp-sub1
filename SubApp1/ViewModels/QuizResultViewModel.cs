namespace SubApp1.ViewModels; // [LEARN] View models namespace

// [LEARN] Overall score and per-question feedback
public class QuizResultViewModel // [LEARN] Result view model
{
    public int QuizId { get; set; } // [LEARN] Quiz id

    public string Title { get; set; } = string.Empty; // [LEARN] Title

    public int CourseId { get; set; } // [LEARN] Course id

    public string CourseTitle { get; set; } = string.Empty; // [LEARN] Course title

    public int AttemptNumber { get; set; } // [LEARN] Attempt number

    public DateTimeOffset CompletedAt { get; set; } // [LEARN] Completed at

    public List<QuizAttemptHistoryViewModel> AttemptHistory { get; set; } = new(); // [LEARN] History

    public decimal PointsEarned { get; set; } // [LEARN] Earned

    public decimal MaximumPoints { get; set; } // [LEARN] Max

    public int CorrectAnswers { get; set; } // [LEARN] Correct count

    public int TotalQuestions { get; set; } // [LEARN] Total count

    public decimal Percentage => MaximumPoints == 0 // [LEARN] Avoid div by zero
        ? 0 // [LEARN] 0
        : Math.Round(PointsEarned / MaximumPoints * 100, 2); // [LEARN] Percentage

    public List<QuizAnswerResultViewModel> Answers { get; set; } = new(); // [LEARN] Answers
}

// [LEARN] Grading feedback for one answer
public class QuizAnswerResultViewModel // [LEARN] Answer result
{
    public string Prompt { get; set; } = string.Empty; // [LEARN] Prompt

    public List<string> Options { get; set; } = new(); // [LEARN] Options

    public string SubmittedAnswer { get; set; } = string.Empty; // [LEARN] Submitted

    public string CorrectAnswer { get; set; } = string.Empty; // [LEARN] Correct

    public bool IsCorrect { get; set; } // [LEARN] Correct?

    public decimal PointsAwarded { get; set; } // [LEARN] Points
}

// [LEARN] Summary shown in the attempt history panel
public class QuizAttemptHistoryViewModel // [LEARN] History item
{
    public int AttemptNumber { get; set; } // [LEARN] Number

    public DateTimeOffset CompletedAt { get; set; } // [LEARN] Completed

    public decimal PointsEarned { get; set; } // [LEARN] Earned

    public decimal MaximumPoints { get; set; } // [LEARN] Max

    public decimal Percentage => MaximumPoints == 0 // [LEARN] Avoid div by zero
        ? 0 // [LEARN] 0
        : Math.Round(PointsEarned / MaximumPoints * 100, 2); // [LEARN] Percentage
}
