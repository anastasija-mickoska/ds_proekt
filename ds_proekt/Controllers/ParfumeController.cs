using ds_proekt.Models;
using ds_proekt.Services;
using ds_proekt.ViewModels;
using Firebase;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ds_proekt.Controllers
{
    public class ParfumeController : Controller
    {
        private readonly FirebaseAuthService _authService;
        private readonly FirebaseParfumeService _firestoreService;

        public ParfumeController(FirebaseAuthService authService, FirebaseParfumeService firestoreService)
        {
            _authService = authService;
            _firestoreService = firestoreService;
        }
        public async Task<ActionResult> AddToCart(string id)
        {
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "User");

            var parfume = await _firestoreService.GetParfumeByIdAsync(id);
            if (parfume == null) return NotFound();

            var cartItem = new CartItem
            {
                UserId = userId,
                ParfumeId = parfume.ParfumeId,
                ParfumeName = parfume.Name,
                Quantity = 1,
                Price = parfume.Price
            };

            var currentOrder = await _firestoreService.GetOrderByUserIdAsync(userId);

            if (currentOrder == null)
            {
                var newOrder = new Order
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    OrderDate = null,
                    IsActive = true, 
                    Items = new List<CartItem> { cartItem },
                    TotalPrice = cartItem.Price
                };

                await _firestoreService.AddOrderAsync(newOrder);
            }
            else
            {
                var existingItem = currentOrder.Items.FirstOrDefault(i => i.ParfumeId == parfume.ParfumeId);
                if (existingItem != null)
                {
                    existingItem.Quantity += 1;
                }
                else
                {
                    currentOrder.Items.Add(cartItem);
                }

                currentOrder.TotalPrice = currentOrder.Items.Sum(i => i.Price * i.Quantity);

                await _firestoreService.UpdateOrderAsync(currentOrder);
            }

            return RedirectToAction("Index", "Order");
        }





        [HttpGet]
        public async Task<IActionResult> Create()
        {
           
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Parfume parfume)
        {
            foreach (var entry in ModelState)
            {
               foreach (var error in entry.Value.Errors)
               {
                   System.Diagnostics.Debug.WriteLine($"Key: {entry.Key}, Error: {error.ErrorMessage}");
                }
            }
            if (!ModelState.IsValid)
            {
                return View(parfume);
            }

            await _firestoreService.AddParfumeAsync(parfume); 
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Index(string searchString)
        {
            var parfumes = await _firestoreService.GetParfumesAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                parfumes = parfumes
                    .Where(p =>
                        (p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                        (p.Brand.Contains(searchString, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return View(parfumes);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            var parfume = await _firestoreService.GetParfumeByIdAsync(id);

            if (parfume == null)
            {
                return NotFound();
            }

            var reviews = await _firestoreService.GetReviewsByProductAsync(id);

            var enrichedReviews = new List<ReviewWithEmail>();

            foreach (var review in reviews)
            {
                var user = await _firestoreService.GetUserByIdAsync(review.UserId);
                enrichedReviews.Add(new ReviewWithEmail
                {
                    Review = review,
                    UserEmail = user?.Email ?? "Unknown"
                });
            }

            var viewModel = new ParfumeDetailsViewModel
            {
                Parfume = parfume,
                Reviews = enrichedReviews
            };

            return View(viewModel);
        }


    }
}

