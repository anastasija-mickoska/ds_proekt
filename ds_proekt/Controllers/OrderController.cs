using Microsoft.AspNetCore.Mvc;

namespace ds_proekt.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
