using Google.Cloud.Firestore;

namespace ds_proekt.Models
{
    [FirestoreData]
    public class CartItem
    {
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public int ParfumeId { get; set; }
        [FirestoreProperty]
        public string ParfumeName { get; set; }
        [FirestoreProperty]
        public int Quantity { get; set; }
        [FirestoreProperty]
        public decimal Price { get; set; }
    }
}
