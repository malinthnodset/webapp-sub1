using Microsoft.AspNetCore.Mvc;
using SubApp1.Services;
using SubApp1.ViewModels;


namespace SubApp1.ViewComponents;

public class SceneViewComponent : ViewComponent
{
    // TODO: replace with the logged-in student's id (same as quiz controller)
    private const int TempStudentId = 1;

    private readonly IFurnitureService _furnitureService;

    public SceneViewComponent(IFurnitureService furnitureService)
    {
        _furnitureService = furnitureService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        
        // Logged out: cat in its box empty room
        if (UserClaimsPrincipal.Identity?.IsAuthenticated != true)
        {
            return View(new SceneViewModel());
        }

        var model = await _furnitureService.GetSceneAsync(TempStudentId);
        return View(model);

    }

}