using SubApp1.Models;
using SubApp1.DAL;
using SubApp1.Services;
using SubApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;

namespace SubApp1.Controllers;

public class QuizController : Controller
{
    // dependency injection - controller looks to interface repository files, and the service
    private readonly IQuizRepository _quizRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IQuizService _quizService;
    private readonly ILogger<QuizController> _logger;

    public QuizController(IQuizRepository quizRepository, IQuizService quizService, ILogger<QuizController> logger, ICourseRepository courseRepository)
    {
        _quizRepository = quizRepository;
        _quizService = quizService;
        _logger = logger;
        _courseRepository = courseRepository;
    }

    // CREATE QUIZ
    [HttpGet] // Creation form
    public async Task<IActionResult> Create()
    {
        var vm = new QuizCreateViewModel();
        // for loop to start with 3 questions upon creation - more can be added in UI
        for (int i = 0; i < 3; i++)
        {
            vm.Questions.Add(new QuestionInput());
        }
        await LoadCourses(vm);
        return View(vm);
    }

    // Submit the form to create the quiz
    [HttpPost]
    public async Task<IActionResult> Create(Quiz quiz)
    {
        var vm = new QuizCreateViewModel();
        if (ModelState.IsValid)
        {
            bool returnOk = await _quizRepository.Create(quiz);
            if (returnOk)
            {
                return RedirectToAction("Index", "Dashboard");  // Index method in Dashboard controller
            }
        }
        // if modelstat invalid or creation in db failed
        _logger.LogWarning("[QuizController] Quiz creation failed {@quiz}", quiz);
        await LoadCourses(vm);       // dropdown list isn't posted back, so this reloads it
        return View(quiz);           // redisplay with the user's input and error messages

        // quiz details
        // var quiz = new Quiz
        // {
        //     Title = vm.Title,
        //     Description = vm.Description,
        //     CourseId = vm.CourseId!.Value,
        //     CreatedByStudentId = 1,      // TODO: replace with logged-in student id when login exists!

        //     // the questions
        //     Questions = vm.Questions.Select((q, i) => new QuizQuestion
        //     {
        //         Prompt = q.Prompt.Trim(), // trims whitespace before grading
        //         CorrectAnswer = q.CorrectAnswer,
        //         Points = q.Points,
        //         Order = i + 1 // to make it start with Q1 instead of Q0
        //     }).ToList()
        // };
    }

    // Used by drop down menu on the create quiz-page
    private async Task LoadCourses(QuizCreateViewModel vm)
    {
        var courses = await _courseRepository.GetAllCourses();
        vm.Courses = new SelectList(courses, "Id", "Name"); // ?
    }

    // TAKE QUIZ
    [HttpGet]
    public async Task<IActionResult> Take(int id) // Loads quiz
    {
        var quiz = await _quizService.GetQuizToTakeAsync(id);
        if (quiz == null)
        {
            _logger.LogError("[QuizController] Quiz id {@quiz} not found", quiz); // Or should it be id?
            return NotFound();
        }
        return View(quiz);
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // ?
    public async Task<IActionResult> Submit(TakeQuizViewModel submission) // Validates, grades, and shows the result
    {
        var quiz = await _quizService.GetQuizToTakeAsync(submission.QuizId);
        if (quiz is null)
        {
            _logger.LogError("[QuizController] Quiz id {@quiz} for submission not found", quiz); // Or should it be id?
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[QuizController] Quiz submission {@submission} failed. Modelstate not valid.", submission);
            foreach (var question in quiz.Questions) // Preserve answers when redisplaying validation errors
            {
                question.SubmittedAnswer = submission.Questions?
                    .FirstOrDefault(answer => answer.QuestionId == question.QuestionId)
                    ?.SubmittedAnswer;
            }

            return View("Take", quiz);
        }

        // TODO: add logged in students ID 
        var studentId = 1;

        var result = await _quizService.GradeQuizAsync(submission, studentId);
        if (result == null)
        {
            _logger.LogWarning("[QuizController] Quiz not found when attempting to submit the attempt {@submission}", submission);
            return BadRequest();
        }
        return View("Result", result);
    }
}