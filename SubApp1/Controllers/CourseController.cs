using SubApp1.DAL;
using Microsoft.AspNetCore.Mvc;
using SubApp1.Models;

namespace SubApp1.Controllers;

public class CourseController : Controller
{
    // dependency injection - controller looks to interface repository files
    private readonly ICourseRepository _courseRepository; // create in DAL
    private readonly ILogger<QuizController> _logger;

    public CourseController(ILogger<QuizController> logger, ICourseRepository courseRepository)
    {
        _logger = logger;
        _courseRepository = courseRepository;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(); // TODO: return course creation page (form)
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (ModelState.IsValid)
        {
            bool returnOk = await _courseRepository.Create(course);
            if (returnOk)
            {
                _logger.LogInformation("New course {@course} was created", course);
                return View(); // TODO: where to return?
            }
        }
        _logger.LogWarning("Course creation failed for course {@course}", course);
        return View(); // TODO: what to return?  
    }

    // TODO: Update course (optional)

    // TODO: Delete course (optional)
}