using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SubApp1.Controllers
{
    [AllowAnonymous]    //Everyone can see this page, also those not logged in
    public class WelcomeController : Controller
    {
    public IActionResult Index()   //GET: to /Welcome/Index
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)    //Skips welcome page if the user is logged in
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();  //shows Views/Welcome/Indz.cshtml
        }
    }
}