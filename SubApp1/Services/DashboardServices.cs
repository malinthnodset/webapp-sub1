using Microsoft.EntityFrameworkCore; // [LEARN] EF Core
using SubApp1.DAL; // [LEARN] Data access
using SubApp1.ViewModels; // [LEARN] View models

// [LEARN] Reads courses, quizzes, questions and the users attempts from the database through QuizDbContext,
// [LEARN] and builds a DashboardViewModel for the DashboardController.

namespace SubApp1.Services; // [LEARN] Services namespace

public class DashboardService // [LEARN] Dashboard service
{
    private readonly QuizDbContext _db; // [LEARN] DB context
    //(constructor) stores the database context that the service uses to query data.
    public DashboardService(QuizDbContext db) => _db = db; // [LEARN] Constructor

    // [LEARN] Loads all courses with their quizzes and questions, finds the users best percentage per quiz from their attempts, and maps everything into DashboardViewModel.
    public async Task<DashboardViewModel> GetDashboardAsync(string? userId) // [LEARN] Build dashboard
    {
        var courses = await _db.Courses // [LEARN] Query courses
            .AsNoTracking() // [LEARN] Read-only
            .Include(c => c.Quizzes) // [LEARN] Include quizzes
                .ThenInclude(q => q.Questions) // [LEARN] Include questions
            .ToListAsync(); // [LEARN] To list

        // [LEARN] Best percentage per quiz for this user (best attempt counts)
        var bestByQuiz = new Dictionary<int, double>(); // [LEARN] Best % per quiz
        if (int.TryParse(userId, out var studentId)) // [LEARN] Parse user id
        {
            var attempts = await _db.QuizAttempts // [LEARN] Query attempts
                .AsNoTracking() // [LEARN] Read-only
                .Include(a => a.Result) // [LEARN] Include result
                .Where(a => a.StudentId == studentId && a.Result != null && a.Result.MaximumPoints > 0) // [LEARN] Filter
                .ToListAsync(); // [LEARN] To list

            bestByQuiz = attempts // [LEARN] Group by quiz
                .GroupBy(a => a.QuizId) // [LEARN] Group
                .ToDictionary(g => g.Key, g => g.Max(a => (double)a.Result!.Percentage)); // [LEARN] Best %
        }

        return new DashboardViewModel // [LEARN] Build view model
        {
            Courses = courses.Select(c => new CourseSummaryViewModel // [LEARN] Map courses
            {
                Id = c.Id, // [LEARN] Id
                Title = c.Name, // [LEARN] Name as title
                Quizzes = c.Quizzes.Select(q => new QuizSummaryViewModel // [LEARN] Map quizzes
                {
                    Id = q.Id, // [LEARN] Id
                    Title = q.Title, // [LEARN] Title
                    QuestionCount = q.Questions.Count, // [LEARN] Count
                    BestPercentage = bestByQuiz.GetValueOrDefault(q.Id, 0) // [LEARN] Best %
                }).ToList() // [LEARN] To list
            }).ToList() // [LEARN] To list
        };
    }
}
