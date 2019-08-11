using Microsoft.AspNetCore.Mvc;

namespace iBDZ.App.Controllers
{
	public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
