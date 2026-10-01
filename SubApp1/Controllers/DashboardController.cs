using Microsoft.AspNetCore.Mvc;
using SubApp1.ViewModels;
// Hard-kodet fake data for synlighetens skyld. Må kobles opp i/etter databasen senere.
namespace SubApp1.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        var model = new DashboardViewModel
        {
            Courses = new List<CourseSummaryViewModel>
            {
                new()
                {
                    Id = 1, Title = "ITPE3200 Web Applications",
                    Quizzes = new List<QuizSummaryViewModel>
                    {
                        new() { Id = 1, Title = "MVC Basics", QuestionCount = 10, BestPercentage = 80 },
                        new() { Id = 2, Title = "Entity Framework", QuestionCount = 8, BestPercentage = 40 }
                    }
                },
                new()
                {
                    Id = 2, Title = "DATA 2000 Databases",
                    Quizzes = new List<QuizSummaryViewModel>
                    {
                        new() { Id = 3, Title = "SQL Joins", QuestionCount = 12, BestPercentage = 0 }
                    }
                }
            }
        };
        return View(model);
    }
}