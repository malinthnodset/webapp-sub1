namespace SubApp1.ViewModels;

public class QuizResultViewModel // Overall score and per-question feedback
{
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int CourseId { get; set; }

    public string CourseTitle { get; set; } = string.Empty;

    public int AttemptNumber { get; set; }

    public DateTimeOffset CompletedAt { get; set; }

    public List<QuizAttemptHistoryViewModel> AttemptHistory { get; set; } = new();

    public decimal PointsEarned { get; set; }

    public decimal MaximumPoints { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public decimal Percentage => MaximumPoints == 0 // Avoid division by zero for an empty quiz
        ? 0
        : Math.Round(PointsEarned / MaximumPoints * 100, 2);

    public List<QuizAnswerResultViewModel> Answers { get; set; } = new();
}

public class QuizAnswerResultViewModel // Grading feedback for one answer
{
    public string Prompt { get; set; } = string.Empty;

    public List<string> Options { get; set; } = new();

    public string SubmittedAnswer { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public decimal PointsAwarded { get; set; }
}

public class QuizAttemptHistoryViewModel // Summary shown in the attempt history panel
{
    public int AttemptNumber { get; set; }

    public DateTimeOffset CompletedAt { get; set; }

    public decimal PointsEarned { get; set; }

    public decimal MaximumPoints { get; set; }

    public decimal Percentage => MaximumPoints == 0
        ? 0
        : Math.Round(PointsEarned / MaximumPoints * 100, 2);
}