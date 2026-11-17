using SubApp1.DAL;
using Microsoft.AspNetCore.Mvc;
using SubApp1.Models;
using Microsoft.AspNetCore.Authorization;
using SubApp1.ViewModels;

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

    // GET: /Course/Create
    [HttpGet]
    [Authorize(Roles = "TEACHER")] // Sørger for at det kun er lærere som kan nå denne siden, til og med når den er direkte skrevet inn i URL.
    public IActionResult Create()
    {
        var vm = new CreateCourseViewModel();
        return View(vm);
    }

    // POST: /Course/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TEACHER")]
    public async Task<IActionResult> Create(CreateCourseViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var course = new Course
            {
                Code = vm.CourseCode,
                Name = vm.CourseName
            };

            bool returnOk = await _courseRepository.Create(course);
            if (returnOk)
            {
                _logger.LogInformation("New course {@course} was created", course);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        _logger.LogWarning("Course creation failed for course {@vm}", vm);
        return View(vm);
    }

    // UPDATE - GET: /Course/Edit
    [HttpGet]
    [Authorize(Roles = "TEACHER")] // Sørger for at det kun er lærere som kan nå denne siden, til og med når den er direkte skrevet inn i URL.
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseRepository.GetCourseById(id);
        if (course == null)
        {
            _logger.LogWarning("Edit requested for course {@id}, but it was not found", id);
            return NotFound();
        }

        var vm = new EditCourseViewModel
        {
            Id = course.Id,
            CourseCode = course.Code,
            CourseName = course.Name
        };
        return View(vm);
    }

    // UPDATE - POST: /Course/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TEACHER")]
    public async Task<IActionResult> Edit(EditCourseViewModel vm)
    {
        if (ModelState.IsValid)
        {
            var course = new Course
            {
                Id = vm.Id,
                Code = vm.CourseCode,
                Name = vm.CourseName
            };
            bool returnOk = await _courseRepository.Update(course);
            if (returnOk)
            {
                _logger.LogInformation("Course {@course} was updated", course);
                return RedirectToAction("Index", "Dashboard");
            }
        }
        _logger.LogWarning("Course update failed for course {@vm}", vm);
        return View(vm);
    }

    // DELETE - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "TEACHER")]
    public async Task<IActionResult> Delete(int id)
    {
        bool returnOk = await _courseRepository.Delete(id);
        if (returnOk)
        {
            _logger.LogInformation("Course {@id} was deleted", id);
            return RedirectToAction("Index", "Dashboard");
        }
        _logger.LogWarning("Course deletion failed for course {@id}", id);
        return RedirectToAction("Index", "Dashboard");
    }
}