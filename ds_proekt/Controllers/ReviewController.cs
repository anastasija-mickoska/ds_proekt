using ds_proekt.Models;
using ds_proekt.Services;
using ds_proekt.ViewModels;
using Firebase;
using FirebaseAdmin;
using Microsoft.AspNetCore.Mvc;
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
            var allReviews = await _firestoreService.GetReviewsAsync();

            // Filter perfumes based on search string
            var matchingParfumeIds = string.IsNullOrWhiteSpace(searchString)
                ? allParfumes.Select(p => p.ParfumeId).ToList()
                : allParfumes
                    .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.ParfumeId)
                    .ToList();

            var filteredReviews = allReviews
                .Where(r => matchingParfumeIds.Contains(r.ParfumeId))
                .ToList();

            var reviewDisplayList = new List<ReviewDisplayViewModel>();

            foreach (var review in filteredReviews)
            {
                var parfume = allParfumes.FirstOrDefault(p => p.ParfumeId == review.ParfumeId);
                var user = await _firestoreService.GetUserByIdAsync(review.UserId);

                var displayReview = new ReviewDisplayViewModel
                {
                    ReviewId = review.ReviewId,
                    ParfumeName = parfume?.Name ?? "Unknown Parfume",
                    UserEmail = user?.Email ?? "Unknown User",
                    Comment = review.Comment,
                    Rating = review.Rating
                };

                reviewDisplayList.Add(displayReview);
            }

            ViewBag.SearchString = searchString;
            return View(reviewDisplayList);
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
