using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

public class FirebaseAuthService
{
    private readonly FirebaseAuthClient _client;

    public FirebaseAuthService()
    {
        var config = new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCnZPqpSzHZYOEp7gPFkpfBuh8P-hZadvg",
            AuthDomain = "ds-proekt-baa0c.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };
        _client = new FirebaseAuthClient(config);
    }

    public async Task<Firebase.Auth.UserCredential> RegisterAsync(ds_proekt.Models.User user)
    {
        return await _client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password);
    }
    public async Task<Firebase.Auth.UserCredential> LoginAsync(string email, string password)
    {
        return await _client.SignInWithEmailAndPasswordAsync(email, password);
    }
    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
    {
        return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
    }
}
