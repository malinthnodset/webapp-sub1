using SubApp1.Models;
using SubApp1.DAL;
using SubApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SubApp1.Controllers;

public class QuizController : Controller
{
    // dependency injection - controller looks to interface repository files
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizService _quizService;

    public QuizController(IQuizRepository quizRepository, IQuizService quizService)
    {
        _quizRepository = quizRepository;
        _quizService = quizService;
    }

    // CREATE QUIZ
    // GET - the creation form
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var view = new QuizCreateViewModel();
        // for loop to start with 3 questions upon creation - more can be added
        for (int i = 0; i < 3; i++) view.Questions.Add(new QuestionInput()); 
        await LoadCourses(view);
        return View(view);
    }

    // POST - submit the form (create the quiz)
    [HttpPost]
    public async Task<IActionResult> Create(QuizCreateViewModel view)
    {
        // must contain >= 1 question
        if (view.Questions.Count == 0)
            ModelState.AddModelError("", "Add at least one question.");

        // error handling if the user tries to submit e.g. empty questions
        if (!ModelState.IsValid)
        {
            await LoadCourses(view);       // dropdown list isn't posted back, so reload it "manually"
            return View(view);             // redisplay with the user's input and error messages
        }

        // quiz details
        var quiz = new Quiz
        {
            Title = view.Title,
            Description = view.Description,
            CourseId = view.CourseId!.Value,
            CreatedByStudentId = 1,      // TODO: replace with logged-in student when login exists!
            
            // the questions
            Questions = view.Questions.Select((q, i) => new QuizQuestion
            {
                Prompt = q.Prompt,
                CorrectAnswer = q.CorrectAnswer,
                Points = q.Points,
                Order = i + 1 // to make it start with Q1 instead of Q0
            }).ToList()
        };

        await _quizRepository.Create(quiz);
        return RedirectToAction(nameof(Index));   // TODO: connect to created dashboard
    }

    private async Task LoadCourses(QuizCreateViewModel vm)
    {
        var courses = await _quizRepository.GetAllCourses();
        vm.Courses = new SelectList(courses, "Id", "Name");
    }

    // TAKE QUIZ
    [HttpGet]
    public IActionResult Take(int id = 1) // Loads one quiz> defaults to the sample quiz
    {
        var quiz = _quizService.GetQuizToTake(id);
        return quiz is null ? NotFound() : View(quiz);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Submit(TakeQuizViewModel submission) // Validates, grades, and shows the result
    {
        var quiz = _quizService.GetQuizToTake(submission.QuizId);
        if (quiz is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            foreach (var question in quiz.Questions) // Preserve answers when redisplaying validation errors
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