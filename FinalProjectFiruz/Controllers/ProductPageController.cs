using Microsoft.AspNetCore.Mvc;

namespace FinalProjectFiruz.Controllers
{
    public class ProductPageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
