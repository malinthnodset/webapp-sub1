using Microsoft.EntityFrameworkCore;
using SubApp1.DAL;
using SubApp1.ViewModels;

namespace SubApp1.Services;

public class DashboardService
{
    private readonly QuizDbContext _db;

    public DashboardService(QuizDbContext db) => _db = db;

    /// <summary>Builds the dashboard: courses, their quizzes, and completion status.</summary>
    public async Task<DashboardViewModel> GetDashboardAsync(string? userId)
    {
        var courses = await _db.Courses
            .AsNoTracking()
            .Include(c => c.Quizzes)
                .ThenInclude(q => q.Questions)
            .ToListAsync();

        // Best percentage per quiz for this user (best attempt counts)
        var bestByQuiz = new Dictionary<int, double>();
        if (int.TryParse(userId, out var studentId))
        {
            var attempts = await _db.QuizAttempts
                .AsNoTracking()
                .Include(a => a.Result)
                .Where(a => a.StudentId == studentId && a.Result != null && a.Result.MaximumPoints > 0)
                .ToListAsync();

            bestByQuiz = attempts
                .GroupBy(a => a.QuizId)
                .ToDictionary(g => g.Key, g => g.Max(a => (double)a.Result!.Percentage));
        }

        return new DashboardViewModel
        {
            Courses = courses.Select(c => new CourseSummaryViewModel
            {
                Id = c.Id,
                Title = c.Name,
                Quizzes = c.Quizzes.Select(q => new QuizSummaryViewModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    QuestionCount = q.Questions.Count,
                    BestPercentage = bestByQuiz.GetValueOrDefault(q.Id, 0)
                }).ToList()
            }).ToList()
        };
    }
}