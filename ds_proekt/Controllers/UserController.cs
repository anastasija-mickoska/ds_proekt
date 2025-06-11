using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace ds_proekt.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    
    public async Task<ActionResult> Verify()
    {
        string idToken = null;

            // Get the ID token from the "Authorization" header
            var authHeader = Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                idToken = authHeader.Substring("Bearer ".Length);
            }

            if (idToken == null)
        {
                return new ObjectResult("No ID token provided") { StatusCode = 401 }; 
        }

        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            string uid = decodedToken.Uid;

            // Now you trust this user
            // You can create a session or return user info
            return Json(new { message = "Authenticated!", uid = uid });
        }
        catch
        {
                return new ObjectResult("No ID token provided") { StatusCode = 401 };
            }
    }

        }
}
