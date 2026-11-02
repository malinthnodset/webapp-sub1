using SubApp1.Models; // [LEARN] Domain entities
using SubApp1.DAL; // [LEARN] Data access
using SubApp1.Services; // [LEARN] Business logic
using SubApp1.ViewModels; // [LEARN] View models
using Microsoft.AspNetCore.Mvc; // [LEARN] MVC
using Microsoft.AspNetCore.Mvc.Rendering; // [LEARN] SelectList for dropdowns

namespace SubApp1.Controllers; // [LEARN] Controller namespace

public class QuizController : Controller // [LEARN] MVC controller for quizzes
{
    // [LEARN] dependency injection - controller looks to interface repository files
    private readonly IQuizRepository _quizRepository; // [LEARN] in DAL
    private readonly ICourseRepository _courseRepository; // [LEARN] in DAL
    private readonly IQuizService _quizService; // [LEARN] in Services
    private readonly ILogger<QuizController> _logger; // [LEARN] Logger

    public QuizController(IQuizRepository quizRepository, IQuizService quizService, ILogger<QuizController> logger, ICourseRepository courseRepository) // [LEARN] Constructor via DI
    {
        _quizRepository = quizRepository; // [LEARN] Store quiz repo
        _quizService = quizService; // [LEARN] Store quiz service
        _logger = logger; // [LEARN] Store logger
        _courseRepository = courseRepository; // [LEARN] Store course repo
    }

    // [LEARN] LIST ALL QUIZZES
    [HttpGet] // [LEARN] Handles GET request
    public async Task<IActionResult> Index() // [LEARN] Show all quizzes
    {
        var quizzes = await _quizRepository.GetAllQuizzes(); // [LEARN] Load all quizzes
        return View(quizzes); // [LEARN] Render Index.cshtml
    }


    // [LEARN] CREATE QUIZ

    // [LEARN] GET - the creation form
    [HttpGet] // [LEARN] Handles GET request
    public async Task<IActionResult> Create() // [LEARN] Show create form
    {
        var vm = new QuizCreateViewModel(); // [LEARN] Empty view model
        // [LEARN] for loop to start with 3 questions upon creation - more can be added
        for (int i = 0; i < 3; i++) vm.Questions.Add(new QuestionInput()); // [LEARN] Seed 3 blank questions
        await LoadCourses(vm); // [LEARN] Load course dropdown
        return View(vm); // [LEARN] Render Create.cshtml
    }

    // [LEARN] POST - submit the form -> create the quiz
    [HttpPost] // [LEARN] Handles POST request
    public async Task<IActionResult> Create(QuizCreateViewModel vm) // [LEARN] Create quiz from form
    {
        // [LEARN] VALIDATION

        // [LEARN] must select an existing course
        if(!vm.CourseId.HasValue) // [LEARN] No course selected
        {
            // [LEARN] no selected course
            ModelState.AddModelError(nameof(vm.CourseId), "Please select a course."); // [LEARN] Add validation error
        }
        else if (!await _courseRepository.CourseExists(vm.CourseId.Value)) // [LEARN] Check course exists
        {
            // [LEARN] course does not exist
            ModelState.AddModelError( // [LEARN] Add validation error
            nameof(vm.CourseId),
            "The selected course does not exist.");
        }

        // [LEARN] quiz must contain >= 1 question
        if (vm.Questions.Count == 0) // [LEARN] No questions
            ModelState.AddModelError("", "Add at least one question."); // [LEARN] Add error

        // [LEARN] error handling if the user tries to submit with an issue - e.g. empty questions
        if (!ModelState.IsValid) // [LEARN] Validation failed
        {
            await LoadCourses(vm); // [LEARN] dropdown list isn't posted back, so this reloads it
            return View(vm);      // [LEARN] redisplay with the user's input and error messages
        }

        // [LEARN] VALIDATION PASSED - QUIZ CREATION

        var quiz = new Quiz // [LEARN] Build Quiz entity
        {
            Title = vm.Title, // [LEARN] Title from form
            Description = vm.Description, // [LEARN] Description from form
            CourseId = vm.CourseId!.Value, // [LEARN] Selected course
            CreatedByStudentId = 1,      // [LEARN] Hard-coded student (TODO: real login)
            // [LEARN] TODO: replace with logged-in student id when login exists!

            Questions = vm.Questions.Select((q, i) => new QuizQuestion // [LEARN] Map question inputs to entities
            {
                Prompt = q.Prompt.Trim(), // [LEARN] trims whitespace before grading
                CorrectAnswer = q.CorrectAnswer, // [LEARN] Correct answer
                Points = q.Points, // [LEARN] Points
                Order = i + 1 // [LEARN] to make it start with Q1 instead of Q0
            }).ToList() // [LEARN] To list
        };

        await _quizRepository.Create(quiz); // [LEARN] Save to DB
        return RedirectToAction("Index", "Dashboard");  // [LEARN] Index method in Dashboard controller
    }
    private async Task LoadCourses(QuizCreateViewModel vm) // [LEARN] Helper to load courses
    {
        var courses = await _courseRepository.GetAllCourses(); // [LEARN] Get all courses
        vm.Courses = new SelectList(courses, "Id", "Name"); // [LEARN] Build dropdown
    }

    // [LEARN] TAKE QUIZ
    [HttpGet] // [LEARN] Handles GET
    public async Task<IActionResult> Take(int id) // [LEARN] Show quiz to take
    {
        var quiz = await _quizService.GetQuizToTakeAsync(id); // [LEARN] Load quiz
        return quiz is null ? NotFound() : View(quiz); // [LEARN] 404 or render Take.cshtml
    }

    // [LEARN] Validate, grade and display results
    [HttpPost] // [LEARN] Handles POST
    [ValidateAntiForgeryToken] // [LEARN] CSRF protection
    public async Task<IActionResult> Submit(TakeQuizViewModel submission) // [LEARN] Submit answers
    {
        var quiz = await _quizService.GetQuizToTakeAsync(submission.QuizId); // [LEARN] Reload quiz
        if (quiz is null) // [LEARN] Unknown quiz
        {
            return NotFound(); // [LEARN] 404
        }

        if (!ModelState.IsValid) // [LEARN] Validation failed
        {
            // [LEARN] Preserve answers when redisplaying validation errors
            foreach (var question in quiz.Questions) // [LEARN] Restore submitted answers
            {
                question.SubmittedAnswer = submission.Questions?
                    .FirstOrDefault(answer => answer.QuestionId == question.QuestionId)
                    ?.SubmittedAnswer; // [LEARN] Match by question id
            }

            return View("Take", quiz); // [LEARN] Redisplay Take view
        }

        // [LEARN] TODO: add logged in students ID 
        var studentId = 1; // [LEARN] Hard-coded student

        var result = await _quizService.GradeQuizAsync(submission, studentId); // [LEARN] Grade quiz
        return result is null ? BadRequest() : View("Result", result); // [LEARN] Render Result or 400
    }
}
