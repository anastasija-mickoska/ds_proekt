using Google.Cloud.Firestore;

namespace ds_proekt.Models
{
    [FirestoreData]
    public class User
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Email { get; set; }
        [FirestoreProperty]
        public string Password { get; set; }

        [FirestoreProperty]
        public string Role { get; set; }

    }
}
