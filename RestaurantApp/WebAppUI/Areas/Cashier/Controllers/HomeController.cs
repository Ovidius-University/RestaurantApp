using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAppUI.Areas.Cashier.Controllers
{
    [Area("Cashier")]
    [Authorize(Roles = "Cashier")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}