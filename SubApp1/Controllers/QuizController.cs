using Microsoft.AspNetCore.Mvc;
using SubApp1.Services;
using SubApp1.ViewModel;

namespace SubApp1.Controllers;

public class QuizController : Controller
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    public IActionResult Take(int id = 1) // GET request 
    {
        var quiz = _quizService.GetQuizToTake(id);
        return quiz is null ? NotFound() : View(quiz);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Submit(TakeQuizViewModel submission) //POST request
    {
        var quiz = _quizService.GetQuizToTake(submission.QuizId);
        if (quiz is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            foreach (var question in quiz.Questions)
            {
                question.SubmittedAnswer = submission.Questions?
                    .FirstOrDefault(answer => answer.QuestionId == question.QuestionId)
                    ?.SubmittedAnswer;
            }

            return View("Take", quiz);
        }

        var result = _quizService.GradeQuiz(submission);
        return result is null ? BadRequest() : View("Result", result);
    }
}