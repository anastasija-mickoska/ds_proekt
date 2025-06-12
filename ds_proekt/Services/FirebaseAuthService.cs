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
            { "Name", user.Name },
            { "Role", user.Role ?? "user" } 
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
    public async Task<Dictionary<string, object>> GetUserDocumentByIdAsync(string uid)
    {
        var doc = await _firestoreDb.Collection("users").Document(uid).GetSnapshotAsync();
        return doc.Exists ? doc.ToDictionary() : new Dictionary<string, object>();
    }
}