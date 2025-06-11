using ds_proekt.Models;
using ds_proekt.Services;
using Microsoft.AspNetCore.Mvc;

namespace ds_proekt.Controllers
{
    public class OrderController : Controller
    {
        private readonly FirebaseParfumeService _firebaseService = new FirebaseParfumeService();
        public IActionResult Index()
        {

            return View();
        }

        public async Task<ActionResult> AddOrder(Order order)
        { 
            await _firebaseService.AddOrderAsync(order);
            return RedirectToAction("Index");
        }
    }
}
