using SubApp1.Models;
using SubApp1.DAL;
using SubApp1.Services;
using SubApp1.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

    /* GOALS (later implementation):
        Create a course - so GET course form and POST the created course
        Update a course
        Delete a course
     */
}