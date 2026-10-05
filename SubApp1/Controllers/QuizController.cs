using SubApp1.Models;
using SubApp1.DAL;
using SubApp1.Services;
using SubApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SubApp1.Controllers;

public class QuizController : Controller
{
    // dependency injection - controller looks to interface repository files
    private readonly IQuizRepository _quizRepository; // in DAL
    private readonly ICourseRepository _courseRepository; // in DAL
    private readonly IQuizService _quizService; // in Services
    private readonly ILogger<QuizController> _logger;

    public QuizController(IQuizRepository quizRepository, IQuizService quizService, ILogger<QuizController> logger, ICourseRepository courseRepository)
    {
        _quizRepository = quizRepository;
        _quizService = quizService;
        _logger = logger;
        _courseRepository = courseRepository;
    }

    // CREATE QUIZ

    // GET - the creation form
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = new QuizCreateViewModel();
        // for loop to start with 3 questions upon creation - more can be added
        for (int i = 0; i < 3; i++) vm.Questions.Add(new QuestionInput()); 
        await LoadCourses(vm);
        return View(vm);
    }

    // POST - submit the form -> create the quiz
    [HttpPost]
    public async Task<IActionResult> Create(QuizCreateViewModel vm)
    {
        // VALIDATION

        // must select an existing course
        if(!vm.CourseId.HasValue)
        {
            // no selected course
            ModelState.AddModelError(nameof(vm.CourseId), "Please select a course.");
        }
        else if (!await _courseRepository.CourseExists(vm.CourseId.Value))
        {
            // course does not exist
            ModelState.AddModelError(
            nameof(vm.CourseId),
            "The selected course does not exist.");
        }

        // quiz must contain >= 1 question
        if (vm.Questions.Count == 0)
            ModelState.AddModelError("", "Add at least one question.");

        // error handling if the user tries to submit with an issue - e.g. empty questions
        if (!ModelState.IsValid)
        {
            await LoadCourses(vm); // dropdown list isn't posted back, so this reloads it
            return View(vm);      // redisplay with the user's input and error messages
        }

        // VALIDATION PASSED - QUIZ CREATION

        var quiz = new Quiz
        {
            Title = vm.Title,
            Description = vm.Description,
            CourseId = vm.CourseId!.Value,
            CreatedByStudentId = 1,      
            // TODO: replace with logged-in student id when login exists!
            
            Questions = vm.Questions.Select((q, i) => new QuizQuestion
            {
                Prompt = q.Prompt.Trim(), // trims whitespace before grading
                CorrectAnswer = q.CorrectAnswer,
                Points = q.Points,
                Order = i + 1 // to make it start with Q1 instead of Q0
            }).ToList()
        };

        await _quizRepository.Create(quiz);
        return RedirectToAction("Index", "Dashboard");  // Index method in Dashboard controller
    }
    private async Task LoadCourses(QuizCreateViewModel vm)
    {
        var courses = await _courseRepository.GetAllCourses();
        vm.Courses = new SelectList(courses, "Id", "Name");
    }

    // TAKE QUIZ
    [HttpGet]
    public async Task<IActionResult> Take(int id)
    {
        var quiz = await _quizService.GetQuizToTakeAsync(id);
        return quiz is null ? NotFound() : View(quiz);
    }

    // Validate, grade and display results
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(TakeQuizViewModel submission) 
    {
        var quiz = await _quizService.GetQuizToTakeAsync(submission.QuizId);
        if (quiz is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            // Preserve answers when redisplaying validation errors
            foreach (var question in quiz.Questions) 
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
        return result is null ? BadRequest() : View("Result", result);
    }
}