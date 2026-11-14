using SubApp1.Models;
using SubApp1.DAL;
using SubApp1.Services;
using SubApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        var vm = new CreateQuizViewModel();
        // for loop to start with 3 questions upon creation - more can be added in UI
        for (int i = 0; i < 3; i++)
        {
            vm.Question.Add(new Question());
        }
        await LoadCourses(vm);
        return View(vm);
    }

    // Submit the form to create the quiz
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateQuizViewModel vm)
    {
        if (vm.CourseId.HasValue && !await _courseRepository.CourseExists(vm.CourseId.Value))
        {
            ModelState.AddModelError(nameof(vm.CourseId), "Choose an existing course.");
        }

        if (ModelState.IsValid)
        {
            var quiz = new Quiz
            {
                Title = vm.Title,
                Description = vm.Description,
                CourseId = vm.CourseId.GetValueOrDefault(),
                CreatedByStudentId = 1,      // TODO: replace with logged-in student id when login exists!

                // the questions
                Questions = vm.Question.Select((q, i) => new QuizQuestion
                {
                    Prompt = q.Prompt.Trim(), // trims whitespace before grading
                    CorrectAnswer = q.CorrectAnswer.Trim(), // trims whitespace of correct answer
                    Points = q.Points,
                    Order = i + 1 // to make it start with Q1 instead of Q0
                }).ToList()
            };

            bool returnOk = await _quizRepository.Create(quiz);
            if (returnOk)
            {
                return RedirectToAction("Index", "Dashboard");  // Index method in Dashboard controller
            }
        }
        // if modelstat invalid or creation in db failed
        _logger.LogWarning("[QuizController] Quiz creation failed {@vm}", vm);
        await LoadCourses(vm);       // dropdown list isn't posted back, so this reloads it
        return View(vm);           // redisplay with the user's input and error messages
    }

    // Helper method - Used by drop down menu on the create quiz-page
    private async Task LoadCourses(CreateQuizViewModel vm)
    {
        var courses = await _courseRepository.GetAllCourses();
        vm.Courses = new SelectList(courses, "Id", "Code");
    }

    // TAKE QUIZ
    [HttpGet]
    public async Task<IActionResult> Take(int id)
    {
        var quiz = await _quizService.GetQuizToTakeAsync(id);
        if (quiz == null)
        {
            _logger.LogError("[QuizController] Quiz id {@quiz} not found", id);
            return NotFound();
        }
        return View(quiz);
    }

    // Validate, grade and display results
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(TakeQuizViewModel submission)
    {
        var quiz = await _quizService.GetQuizToTakeAsync(submission.QuizId);
        if (quiz is null)
        {
            _logger.LogError("[QuizController] Quiz id {QuizId} for submission not found", submission.QuizId);
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("[QuizController] Quiz submission {QuizId} failed. Modelstate not valid.", submission.QuizId);

            // Preserve answers when reloading quiz to display validation errors
            foreach (var question in quiz.Questions)
            {
                // find a submission with the same question id as the quiz' question id
                var match = submission.Questions? // make the whole expression null if there's nothing posted
                    .FirstOrDefault(answer => answer.QuestionId == question.QuestionId);

                // if match != null, store the answer so it can be redisplayed
                question.SubmittedAnswer = match?.SubmittedAnswer;
            }

            return View("Take", quiz);
        }

        // If modelstate is valid, save and grade the attempt:

        // TODO: add logged in students ID 
        var studentId = 1;

        var result = await _quizService.GradeQuizAsync(submission, studentId);
        if (result == null)
        {
            _logger.LogWarning("[QuizController] Invalid submission for quiz {QuizId}", submission);
            return BadRequest();
        }
        return View("Result", result);
    }

    //Hvorfor er dette kommentert ut? (før denne implementeres sjekk koden under om den er lik!)
    //SHOW RESULT - TODO: implement GetQuizResultAsync(attemptId)
    //[HttpGet] // Creation form
    //            public async Task<IActionResult> Result(int attemptId)
    //{
    //    var result = await _quizService.GetQuizResultAsync(attemptId);
    //    if (result == null)
    //    {
    //        _logger.LogError("[QuizController] Quiz attempt id {AttemptId} not found", attemptId);
    //        return NotFound();
    //    }
    //    return View(result);
    //}

// Dashboard: SHOW LATEST RESULT: open past results without retaking the quiz
[HttpGet]
public async Task<IActionResult> Result(int id)
{
    var studentId = 1; // TODO: same logged-in student id as in Submit
    var result = await _quizService.GetLatestResultAsync(id, studentId);
    if (result == null)
    {
        _logger.LogWarning("[QuizController] No attempts found for quiz {QuizId}", id);
        return RedirectToAction("Take", new { id });   // no attempt yet -> take the quiz
    }
    return View(result);
}


}