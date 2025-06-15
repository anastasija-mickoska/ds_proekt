using ds_proekt.Models;
using ds_proekt.Services;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;
using Firebase;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ds_proekt.Controllers
{
    public class ReviewController : Controller
    
    {
        private readonly FirebaseAuthService _authService;
        private readonly FirebaseParfumeService _firestoreService;

        public ReviewController(FirebaseAuthService authService, FirebaseParfumeService firestoreService)
        {
            _authService = authService;
            _firestoreService = firestoreService;
        }
        public async Task<ActionResult> Index(string searchString)
        {
            var allParfumes = await _firestoreService.GetParfumesAsync();

            var matchingParfumeIds = string.IsNullOrWhiteSpace(searchString)
                ? allParfumes.Select(p => p.ParfumeId).ToList()
                : allParfumes
                    .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.ParfumeId)
                    .ToList();

            var allReviews = await _firestoreService.GetReviewsAsync();

            var filteredReviews = allReviews
                .Where(r => matchingParfumeIds.Contains(r.ParfumeId))
                .ToList();

            ViewBag.SearchString = searchString;

            return View(filteredReviews);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Review review)
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
                var parfumes = await _firestoreService.GetParfumesAsync();
                ViewBag.ParfumeId = new SelectList(parfumes, "ParfumeId", "Name");
                return View(review); 
            }
            string userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            review.UserId = userId;

            await _firestoreService.AddReviewAsync(review);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var parfumes = await _firestoreService.GetParfumesAsync();
            ViewBag.ParfumeId = new SelectList(parfumes, "ParfumeId", "Name");
            return View();
        }

    }
}
