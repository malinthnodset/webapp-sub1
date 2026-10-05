namespace SubApp1.ViewModels;

public class DashboardViewModel
{
    public List<CourseSummaryViewModel> Courses { get; set; } = new();
}

// Course-info
public class CourseSummaryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int QuizCount => Quizzes.Count;
    public bool IsCompleted => Quizzes.Count > 0 && Quizzes.All(q => q.IsCompleted);
    public List<QuizSummaryViewModel> Quizzes { get; set; } = new();
}

// Quiz info
public class QuizSummaryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int QuestionCount { get; set; }
    public double BestPercentage { get; set; }   // 0-100
    public bool IsCompleted => BestPercentage >= 60;
}
