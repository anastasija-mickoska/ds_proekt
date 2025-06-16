using ds_proekt.Models;
using ds_proekt.Services;
using ds_proekt.ViewModels;
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
            string userId = HttpContext.Session.GetString("UserId");
            string role = HttpContext.Session.GetString("UserRole");
            string email = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var allOrders = await _firestoreService.GetOrdersAsync();

            var viewModelList = new List<OrderDisplayViewModel>();

            foreach (var order in allOrders)
            {
                if (order.Items != null)
                {
                    var user = await _firestoreService.GetUserByIdAsync(order.UserId);

                    var vm = new OrderDisplayViewModel
                    {
                        OrderId = order.Id,
                        UserEmail = user?.Email ?? "Unknown",
                        Items = order.Items,
                        OrderDate = order.OrderDate,
                        TotalPrice = order.TotalPrice
                    };

                    viewModelList.Add(vm);
                }
            }

            ViewData["UserRole"] = role;

            if (role == "admin")
            {
                return View(viewModelList);
            }
            else
            {
                var userOrders = viewModelList.Where(o => o.UserEmail == email && o.OrderDate == null).ToList();
                return View(userOrders);
            }
        }


        [HttpGet]
        public async Task<IActionResult> DeleteFromCart(string id)
        {
            string userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }
            var currentOrder = await _firestoreService.GetOrderByUserIdAsync(userId);
            System.Diagnostics.Debug.WriteLine(currentOrder.Id, currentOrder.Items, currentOrder.OrderDate);
            if (currentOrder == null || currentOrder.Items == null)
            {
                return NotFound("Order or items not found");
            }
            var itemToRemove = currentOrder.Items.FirstOrDefault(i => i.ParfumeId == id);
            if (itemToRemove == null)
            {
                return NotFound("Item not found in your cart");
            }

            currentOrder.Items.Remove(itemToRemove);
            currentOrder.TotalPrice = currentOrder.Items.Sum(i => i.Price * i.Quantity);

            await _firestoreService.UpdateOrderAsync(currentOrder);

            return RedirectToAction("Index");
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
            order.Id = userOrder.Id;
            if (userOrder == null)
            {
                return BadRequest("No active order found for user.");
            }
            order.Items = userOrder.Items;
            order.TotalPrice = userOrder.Items.Sum(i => i.Quantity * i.Price);
            order.IsActive = false;
            await _firestoreService.UpdateOrderAsync(order);

            return RedirectToAction("CompleteOrder", new { id = order.Id });
        }

        public async Task<IActionResult> CompleteOrder(string orderId)
        {
            var order = await _firestoreService.GetOrderByIdAsync(orderId); 
            if (order == null)
            {
                return NotFound();
            }

            return View(order); 
        }


    }
}


