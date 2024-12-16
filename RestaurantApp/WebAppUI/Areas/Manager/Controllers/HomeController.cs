using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppUI.Areas.Manager.Controllers;

[Area("Manager")]
[Authorize]
//[Authorize(Roles = "Manager")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
