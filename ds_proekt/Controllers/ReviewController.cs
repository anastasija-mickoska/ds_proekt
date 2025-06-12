using ds_proekt.Models;
using ds_proekt.Services;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;
using Firebase;

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

            var filteredParfumes = string.IsNullOrWhiteSpace(searchString)
                ? allParfumes
                : allParfumes
                    .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            ViewBag.SearchString = searchString;

            return View(filteredParfumes);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> Create([FromHeader(Name = "Authorization")] string idToken, [FromBody] Review review)
        {
            if (string.IsNullOrEmpty(idToken)) return Unauthorized("Missing token");

            var decodedToken = await _authService.VerifyIdTokenAsync(idToken.Replace("Bearer ", ""));
            if (decodedToken == null) return Unauthorized("Invalid token");

            string uid = decodedToken.Uid;

            review.UserId = uid;

            await _firestoreService.AddReviewAsync(review);
            return Ok("Review submitted");
        }
    }
}
