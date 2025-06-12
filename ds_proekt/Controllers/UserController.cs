using Microsoft.AspNetCore.Mvc;
using ds_proekt.Models;
using ds_proekt.Services; 
using Microsoft.AspNetCore.Http;

namespace ds_proekt.Controllers
{
    public class UserController : Controller
    {
        private readonly FirebaseAuthService _authService;

        public UserController(FirebaseAuthService authService)
        {
            _authService = authService;
        }

        // REGISTER
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            try
            {
                var result = await _authService.RegisterAsync(user);

                // Optionally: Store additional user data to Realtime DB or Firestore here

                TempData["Message"] = "Registration successful!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration failed: {ex.Message}");
                return View(user);
            }
        }

        // LOGIN
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                var authLink = await _authService.LoginAsync(email, password);

                var idToken = await authLink.User.GetIdTokenAsync();

                HttpContext.Session.SetString("FirebaseToken", idToken);
                HttpContext.Session.SetString("UserId", authLink.User.Uid);

                return RedirectToAction("Index","Parfume");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Login failed: " + ex.Message);
                return View();
            }
        }

        //// PROFILE
        //public IActionResult Profile()
        //{
        //    var userEmail = HttpContext.Session.GetString("UserEmail");
        //    if (string.IsNullOrEmpty(userEmail))
        //        return RedirectToAction("Login");

        //    ViewBag.Email = userEmail;
        //    ViewBag.UserId = HttpContext.Session.GetString("UserId");

        //    return View();
        //}

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
