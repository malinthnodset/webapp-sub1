namespace SubApp1.ViewModel;

public class QuizResultViewModel
{
    public string Title { get; set; } = string.Empty;

    public decimal PointsEarned { get; set; }

    public decimal MaximumPoints { get; set; }

    public int CorrectAnswers { get; set; }

    public int TotalQuestions { get; set; }

    public decimal Percentage => MaximumPoints == 0
        ? 0
        : Math.Round(PointsEarned / MaximumPoints * 100, 2);

    public List<QuizAnswerResultViewModel> Answers { get; set; } = new();
}

public class QuizAnswerResultViewModel
{
    public string Prompt { get; set; } = string.Empty;

    public string SubmittedAnswer { get; set; } = string.Empty;

    public string CorrectAnswer { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public decimal PointsAwarded { get; set; }
}