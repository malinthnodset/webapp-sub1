namespace SubApp1.ViewModels; // [LEARN] View models namespace

// [LEARN] Defines the data shapes (DashboardViewModel, CourseSummaryViewModel, QuizSummaryViewModel) '
// [LEARN] that carry the dashboard data from the service to the view, including the "checkmarks" completion logic


// [LEARN] Holds the list of courses passed to the dashboard view.
public class DashboardViewModel // [LEARN] Dashboard view model
{
    public List<CourseSummaryViewModel> Courses { get; set; } = new(); // [LEARN] Courses list
}

// [LEARN] Course-info
public class CourseSummaryViewModel // [LEARN] Course summary
{
    public int Id { get; set; } // [LEARN] Id
    public string Title { get; set; } = ""; // [LEARN] Title
    // [LEARN] returns the number of quizzes in the course
    public int QuizCount => Quizzes.Count; // [LEARN] Quiz count
    // [LEARN] returns true if the course has at least one quiz and all quizzes are completed
    public bool IsCompleted => Quizzes.Count > 0 && Quizzes.All(q => q.IsCompleted); // [LEARN] Completed?
    public List<QuizSummaryViewModel> Quizzes { get; set; } = new(); // [LEARN] Quizzes list
}

// [LEARN] Quiz info
public class QuizSummaryViewModel // [LEARN] Quiz summary
{
    public int Id { get; set; } // [LEARN] Id
    public string Title { get; set; } = ""; // [LEARN] Title
    public int QuestionCount { get; set; } // [LEARN] Question count
    public double BestPercentage { get; set; }   // [LEARN] 0-100
    // [LEARN] returns true if the best percentage is 60 or higher
    public bool IsCompleted => BestPercentage >= 60; // [LEARN] Completed?
}
