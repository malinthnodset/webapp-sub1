using Microsoft.AspNetCore.Mvc; // [LEARN] MVC framework
using SubApp1.Services; // [LEARN] Business logic
using SubApp1.ViewModels; // [LEARN] Data passed to views

// [LEARN] Handles the Dashboard page by asking DashboardService for data and passing result to Index.cshtml.

namespace SubApp1.Controllers; // [LEARN] Namespace for controllers
//(constructor) receives the DashboardService and logger through dependency injection, 
// and stores them in private fields for use in the Index action method.
public class DashboardController : Controller // [LEARN] MVC controller for dashboard
{
    private readonly DashboardService _service; // [LEARN] Service field
    private readonly ILogger<DashboardController> _logger; // [LEARN] Logger field

    public DashboardController(DashboardService service, ILogger<DashboardController> logger) // [LEARN] Constructor via DI
    {
        _service = service; // [LEARN] Store service
        _logger = logger; // [LEARN] Store logger
    }

    // [LEARN] Asks for the service for the dashboard data, and passes it to the view. If an error occurs, logs it and shows an error message.
    public async Task<IActionResult> Index() // [LEARN] GET /Dashboard/Index
    {
        try // [LEARN] Try to load dashboard
        {
            var userId = "demo-user"; // [LEARN] Hard-coded demo user (TODO: real login)
            // [LEARN] TODO: replace with the logged-in user's id
            var model = await _service.GetDashboardAsync(userId); // [LEARN] Build view model
            return View(model); // [LEARN] Render Index.cshtml with model
        }
        catch (Exception ex) // [LEARN] Handle errors
        {
            _logger.LogError(ex, "Failed to load dashboard"); // [LEARN] Log error
            TempData["Error"] = "Could not load the dashboard. Please try again."; // [LEARN] Flash error message
            return View(new DashboardViewModel()); // [LEARN] Render empty dashboard
        }
    }
}
