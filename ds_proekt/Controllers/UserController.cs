using ds_proekt.Models;
using ds_proekt.Services; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

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
            {
                //foreach (var entry in ModelState)
                //{
                //    foreach (var error in entry.Value.Errors)
                //    {
                //        System.Diagnostics.Debug.WriteLine($"Key: {entry.Key}, Error: {error.ErrorMessage}");
                //    }
                //}
                return View(user);
            }

            try
            {
                var result = await _authService.RegisterAsync(user);

                TempData["Message"] = "Registration successful!";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration failed: {ex.Message}");
                Console.WriteLine(ex.Message);
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

                if (authLink == null || authLink.User == null)
                {
                    ModelState.AddModelError("", "Login failed: Unable to authenticate.");
                    return View();
                }

                var userDoc = await _authService.GetUserDocumentByIdAsync(authLink.User.Uid);
                string role = userDoc.ContainsKey("Role") ? userDoc["Role"].ToString() : "user";

                HttpContext.Session.SetString("FirebaseToken", await authLink.User.GetIdTokenAsync());
                HttpContext.Session.SetString("UserId", authLink.User.Uid);
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserEmail", email);

                return RedirectToAction("Index", "Parfume");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login error: {ex.Message}");
                return View();
            }
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

