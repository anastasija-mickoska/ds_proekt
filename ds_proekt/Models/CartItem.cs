using Google.Cloud.Firestore;

namespace ds_proekt.Models
{
    [FirestoreData]
    public class CartItem
    {
        [FirestoreProperty]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public string ParfumeId { get; set; }
        [FirestoreProperty]
        public string ParfumeName { get; set; }
        [FirestoreProperty]
        public int Quantity { get; set; }
        [FirestoreProperty]
        public double Price { get; set; }
    }
}
