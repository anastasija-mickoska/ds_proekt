using Firebase.Auth;
using FirebaseAdmin.Auth;

public class FirebaseAuthService
{
    private readonly string _apiKey = "YOUR_FIREBASE_API_KEY";
    private readonly FirebaseAuthProvider _authProvider;

    public FirebaseAuthService()
    {
        _authProvider = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
    }

    public async Task<Firebase.Auth.User> RegisterUserAsync(string email, string password)
    {
        var result = await _authProvider.CreateUserWithEmailAndPasswordAsync(email, password);
        return result.User;
    }

    public async Task<FirebaseAuthLink> LoginUserAsync(string email, string password)
    {
        return await _authProvider.SignInWithEmailAndPasswordAsync(email, password);
    }

    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
    {
        return await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
    }
}
