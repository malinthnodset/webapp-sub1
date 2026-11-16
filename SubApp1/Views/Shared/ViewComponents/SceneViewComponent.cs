using Microsoft.AspNetCore.Mvc;
using SubApp1.ViewModels;

namespace SubApp1.ViewComponents;

// A view component is a small controller + view that can be placed inside a layout.
// The layout has no ViewModel of its own, so this is how it gets data.
public class SceneViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        // TEMPORARY test data, replaced with real data in step 8
        var model = new SceneViewModel
        {
            CatOutOfBox = false,
            FurnitureSlugs = new List<string> { "cat-tree", "bed", "plant", "yarn", "painting-mona-lisa", "painting-wave" }
        };
        return Task.FromResult<IViewComponentResult>(View(model));
    }
}