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
//using ds_proekt.ViewModels;

namespace ds_proekt.Controllers
{
    public class ParfumeController : Controller
    {
        private readonly FirebaseParfumeService _firebaseService = new FirebaseParfumeService();
        public async Task<ActionResult> AddToCart(int id)
        {
            var parfume = await _firebaseService.GetParfumeByIdAsync(id);

            if (parfume == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                ParfumeId = parfume.ParfumeId,
                Quantity = 1,
                TotalPrice = parfume.Price

            };

            await _firebaseService.AddToCartAsync(cartItem);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Create(Parfume parfume)
        {
            if (ModelState.IsValid)
            {
                await _firebaseService.AddParfumeAsync(parfume);
                return RedirectToAction("Index");
            }
            return View(parfume);
        }

        public async Task<ActionResult> Index(string searchString)
        {
            var parfumes = await _firebaseService.GetParfumesAsync();

            if (!String.IsNullOrEmpty(searchString))
            {
                parfumes = parfumes
                    .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(parfumes);
        }
        public async Task<ActionResult> Details(int id)
        {
            var parfume = await _firebaseService.GetParfumeByIdAsync(id);

            if (parfume == null)
            {
                return NotFound();
            }

            var reviews = await _firebaseService.GetReviewsByProductAsync(id);

            var viewModel = new ParfumeDetailsViewModel
            {
                Parfume = parfume,
                Reviews = reviews
            };

            return View(viewModel);
        }

    }
}
