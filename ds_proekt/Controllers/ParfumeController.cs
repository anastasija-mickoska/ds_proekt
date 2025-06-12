using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ds_proekt.Models;
using FirebaseAdmin;
using Firebase;
using ds_proekt.Services;

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
            var parfume = await _firestoreService.GetParfumeByIdAsync(id);

            if (parfume == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                ParfumeId = parfume.ParfumeId,
                ParfumeName = parfume.Name,
                Quantity = 1,
                Price = parfume.Price

            };

            await _firestoreService.AddToCartAsync(cartItem);
            return RedirectToAction("Index", "Order");
        }

        public async Task<ActionResult> Create(Parfume parfume)
        {
            if (ModelState.IsValid)
            {
                await _firestoreService.AddParfumeAsync(parfume);
                return RedirectToAction("Index");
            }
            return View(parfume);
        }

        public async Task<ActionResult> Index(string searchString)
        {
            var parfumes = await _firestoreService.GetParfumesAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                parfumes = parfumes
                    .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(parfumes);
        }
        public async Task<ActionResult> Details(string id)
        {
            var parfume = await _firestoreService.GetParfumeByIdAsync(id);

            if (parfume == null)
            {
                return NotFound();
            }

            var reviews = await _firestoreService.GetReviewsByProductAsync(id);

            var viewModel = new ParfumeDetailsViewModel
            {
                Parfume = parfume,
                Reviews = reviews
            };

            return View(viewModel);
        }

    }
}
