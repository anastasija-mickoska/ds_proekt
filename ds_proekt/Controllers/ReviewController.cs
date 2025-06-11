using ds_proekt.Models;
using ds_proekt.Services;
using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;
using Firebase;
using ds_proekt.Services;

namespace ds_proekt.Controllers
{
    public class ReviewController : Controller
    
    {
        private readonly FirebaseParfumeService _firebaseService = new FirebaseParfumeService();
        public async Task<ActionResult> Index(string searchString)
        {
     
            var allParfumes = await _firebaseService.GetParfumesAsync();

         
            var matchingParfumeIds = string.IsNullOrWhiteSpace(searchString)
                ? allParfumes.Select(p => p.ParfumeId).ToList()
                : allParfumes
                    .Where(p => p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .Select(p => p.ParfumeId)
                    .ToList();

            
            var allReviews = await _firebaseService.GetReviewsAsync(); 

           
            var filteredReviews = allReviews
                .Where(r => matchingParfumeIds.Contains(r.ParfumeId))
                .ToList();

            ViewBag.SearchString = searchString;

            return View(filteredReviews);
        }

        public async Task<ActionResult> Create(Review review)
        {
            if (ModelState.IsValid)
            {
                await _firebaseService.AddReviewAsync(review);
                return RedirectToAction("Index");
            }
            return View(review);
        }
    }
}
