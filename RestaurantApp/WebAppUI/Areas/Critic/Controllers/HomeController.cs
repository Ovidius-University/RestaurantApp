using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppUI.Areas.Critic.Controllers;

[Area("Critic")]
[Authorize]
//[Authorize(Roles = "Critic")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
