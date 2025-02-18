using Microsoft.AspNetCore.Mvc;

namespace StocksSimulator.Server.Controllers
{
    public class HoldingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
