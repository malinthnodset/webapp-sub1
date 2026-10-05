//Handles the "Dashboard page" by asking DashboardService for the data and passing the result to the Index.cshtml.
using Microsoft.AspNetCore.Mvc;
using SubApp1.Services;
using SubApp1.ViewModels;

namespace SubApp1.Controllers;

public class DashboardController : Controller
{
    private readonly DashboardService _service;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(DashboardService service, ILogger<DashboardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = "demo-user"; // TODO: replace with the logged-in user's id
            var model = await _service.GetDashboardAsync(userId);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load dashboard");
            TempData["Error"] = "Could not load the dashboard. Please try again.";
            return View(new DashboardViewModel());
        }
    }
}