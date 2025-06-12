using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using ds_proekt.Models;

public class FirebaseAuthService
{
    private readonly FirebaseAuthClient _client;
    private readonly FirestoreDb _firestoreDb;

    static FirebaseAuthService()
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("../ds-proekt-baa0c-firebase-adminsdk-fbsvc-a3c65714d0.json")
            });
        }
    }

    public FirebaseAuthService()
    {
        _firestoreDb = FirestoreDb.Create("ds-proekt-baa0c");

        _client = new FirebaseAuthClient(new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyCnZPqpSzHZYOEp7gPFkpfBuh8P-hZadvg",
            AuthDomain = "ds-proekt-baa0c.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        });
    }
    public async Task<Firebase.Auth.UserCredential> RegisterAsync(ds_proekt.Models.User user)
    {
        var userCredential = await _client.CreateUserWithEmailAndPasswordAsync(user.Email, user.Password);
        string uid = userCredential.User.Uid;

        var userData = new Dictionary<string, object>
        {
            { "Email", user.Email },
            { "Name", user.Name }
        };

        DocumentReference docRef = _firestoreDb.Collection("users").Document(uid);
        await docRef.SetAsync(userData);

        return userCredential;
    }

    public async Task<Firebase.Auth.UserCredential> LoginAsync(string email, string password)
    {
        return await _client.SignInWithEmailAndPasswordAsync(email, password);
    }
    public async Task<FirebaseToken?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        }
        catch
        {
            return null;
        }
    }
}