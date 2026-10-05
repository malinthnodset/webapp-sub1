using SubApp1.DAL;
using Microsoft.AspNetCore.Mvc;

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

    /* TODO:
        Get creation form
        Create course
        Update course
        Delete course
     */
}