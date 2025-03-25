using Microsoft.AspNetCore.Mvc;

namespace DevoteWebsite.Controllers
{
    public class PlayController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
