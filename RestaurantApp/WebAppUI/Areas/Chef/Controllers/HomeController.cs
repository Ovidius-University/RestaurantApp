using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppUI.Areas.Chef.Controllers;

[Area("Chef")]
[Authorize]
//[Authorize(Roles = "Chef")]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
