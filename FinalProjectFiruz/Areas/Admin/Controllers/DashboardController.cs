using Microsoft.AspNetCore.Mvc;

namespace FinalProjectFiruz.Areas.Admin.Controllers;

[Area("Admin")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
