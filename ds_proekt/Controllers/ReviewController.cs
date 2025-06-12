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

        [HttpGet]
        public async Task<IActionResult > Create()

        {
            var products = await _firestoreService.GetParfumesAsync();

            var selectList = products.Select(p => new SelectListItem
            {
                Value = p.ParfumeId,
                Text = p.Name
            }).ToList();

            ViewData["ParfumeId"] = selectList;
            return View();
        }
    }
}
