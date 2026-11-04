namespace SubApp1.Controllers;

using Microsoft.AspNetCore.Mvc;

// applies to students and professors, shared methods
public class UserController : Controller
{
    private readonly IUserRepository _db;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserRepository db, ILogger<UserController> logger)
    {
        _db = db;
        _logger = logger;
    }


    // GET - registration form
    public IActionResult Create()
    {
        return View();
    }

    // Create user
    public async Task<IActionResult> Create(User user) { }

    // Update user-information
    public async Task<IActionResult> Update(User user) { }

    // Delete user
    public async Task<IActionResult> Delete(int id) { }
}