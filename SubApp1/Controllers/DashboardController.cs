using Microsoft.AspNetCore.Mvc;
using SubApp1.Services;
using SubApp1.ViewModels;

//Handles the "Dashboard page" by asking DashboardService for the data and passing the result to the Index.cshtml.

namespace SubApp1.Controllers;
//(constructor) receives the DashboardService and logger through dependency injection, 
// and stores them in private fields for use in the Index action method.
public class DashboardController : Controller
{
    private readonly DashboardService _service;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(DashboardService service, ILogger<DashboardController> logger)
    {
        _service = service;
        _logger = logger;
    }

    //Asks for the service for the dashboard data, and passes it to the view. If an error occurs, logs it and shows an error message.
    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = "demo-user"; 
            // TODO: replace with the logged-in user's id
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