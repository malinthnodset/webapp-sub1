//Defines the data shapes (DashboardViewModel, CourseSummaryViewModel, QuizSummaryViewModel) that carry the dashboard data from the service to the view, including the "checkmarks" completion logic
namespace SubApp1.ViewModels;
//Holds the list of courses passed to the dashboard view.
public class DashboardViewModel 
{
    public List<CourseSummaryViewModel> Courses { get; set; } = new();
}

// Course-info
public class CourseSummaryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    //returns the number of quizzes in the course
    public int QuizCount => Quizzes.Count;
    //returns true if the course has at least one quiz and all quizzes are completed
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
    //returns true if the best percentage is 60 or higher
    public bool IsCompleted => BestPercentage >= 60;
}
