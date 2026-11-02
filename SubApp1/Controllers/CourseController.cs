using SubApp1.DAL; // [LEARN] Data access layer (repository interfaces)
using Microsoft.AspNetCore.Mvc; // [LEARN] ASP.NET Core MVC framework

namespace SubApp1.Controllers; // [LEARN] Group controllers in this namespace

public class CourseController : Controller // [LEARN] MVC controller for course-related pages
{
    // [LEARN] dependency injection - controller looks to interface repository files
    private readonly ICourseRepository _courseRepository; // [LEARN] create in DAL
    private readonly ILogger<QuizController> _logger; // [LEARN] Logger for this controller

    public CourseController(ILogger<QuizController> logger, ICourseRepository courseRepository) // [LEARN] Constructor: DI provides logger + repo
    {
        _logger = logger; // [LEARN] Store logger
        _courseRepository = courseRepository; // [LEARN] Store repository
    }

    /* TODO:
        Get creation form
        Create course
        Update course
        Delete course
     */ // [LEARN] Not implemented yet
}
