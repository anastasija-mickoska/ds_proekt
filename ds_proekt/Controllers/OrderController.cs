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
            var allOrders = await _firestoreService.GetOrdersAsync();
            var allUsers = await _firestoreService.GetUsersAsync(); 
            var allParfumes = await _firestoreService.GetParfumesAsync();

            string userId = HttpContext.Session.GetString("UserId");
            string role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var viewModelList = new List<OrderDisplayViewModel>();

            if (role == "admin")
            {
                foreach (var order in allOrders)
                {
                    if (order.Items != null)
                    {
                        //foreach (var item in order.Items)
                        //{
                        var user = await _firestoreService.GetUserByIdAsync(order.UserId);
                            //var parfume = allParfumes.FirstOrDefault(p => p.ParfumeId == item.ParfumeId);

                            viewModelList.Add(new OrderDisplayViewModel
                            {
                                OrderId = order.Id,
                                UserEmail = user?.Email ?? "Unknown",
                                //ParfumeName = parfume?.Name ?? "Unknown",
                                //Quantity = item.Quantity,
                                //Price = item.Price,
                                OrderDate = order.OrderDate,
                                TotalPrice = order.TotalPrice
                            });
                      //  }
                    }
                }

                ViewData["UserRole"] = "admin";
                return View(viewModelList);
            }
            else
            {
                var userOrder = allOrders.FirstOrDefault(o => o.UserId == userId && o.IsActive == true);

                if (userOrder != null && userOrder.Items != null)
                {
                    foreach (var item in userOrder.Items)
                    {
                        var parfume = allParfumes.FirstOrDefault(p => p.ParfumeId == item.ParfumeId);

                        viewModelList.Add(new OrderDisplayViewModel
                        {
                           // OrderId = userOrder.Id,
                           // UserEmail = ""
                            Items = new List<CartItem>
                            {
                                new CartItem
                                {
                                    ParfumeName = parfume?.Name ?? "Unknown",
                                    Quantity = item.Quantity,
                                    Price = item.Price
                                }
                            },
                          //  OrderDate = userOrder.OrderDate,
                            TotalPrice = userOrder.TotalPrice
                        });
                    }
                }

                ViewData["UserRole"] = role;
                return View(viewModelList);
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
            order.Id = userOrder.Id;
            order.Items = userOrder.Items;
            order.TotalPrice = userOrder.Items.Sum(i => i.Quantity * i.Price);
            order.IsActive = false;
            await _firestoreService.UpdateOrderAsync(order);

            return RedirectToAction("Index", "Home");
        }



    }
}


