using ds_proekt.Models;
using ds_proekt.Services;
using Microsoft.AspNetCore.Mvc;

namespace ds_proekt.Controllers
{
    public class OrderController : Controller
    {
        private readonly FirebaseAuthService _authService;
        private readonly FirebaseParfumeService _firestoreService;

        public OrderController(FirebaseAuthService authService, FirebaseParfumeService firestoreService)
        {
            _authService = authService;
            _firestoreService = firestoreService;
        }
        public async Task<IActionResult> Index()
        {
            var allOrders = await _firestoreService.GetOrdersAsync();
            string userId = HttpContext.Session.GetString("UserId");
            string role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                if (role == "admin")
                {
                    ViewData["UserRole"] = role;
                    return View(allOrders);
                }
                else
                {
                    var userOrder = allOrders.FirstOrDefault(o => o.UserId == userId && o.OrderDate == null);
                    ViewData["UserRole"] = role;
                    return View(userOrder);
                }
            }
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View(); 
        }

        [HttpPost]
        public async Task<ActionResult> AddOrder(Order order)
        {
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            order.UserId = userId;
            order.OrderDate = DateTime.UtcNow;
            var userOrder = await _firestoreService.GetOrderByUserIdAsync(userId);
            order.Items = userOrder.Items;
            order.TotalPrice = userOrder.Items.Sum(i => i.Quantity * i.Price); 

            await _firestoreService.AddOrderAsync(order);

            return RedirectToAction("Index", "Home");
        }



    }
}


