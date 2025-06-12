using Google.Cloud.Firestore;

namespace ds_proekt.Models
{
    [FirestoreData]
    public class Order
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public DateTime? OrderDate { get; set; }
        [FirestoreProperty]
        public decimal TotalPrice { get; set; }
        [FirestoreProperty]
        public List<CartItem> Items { get; set; }
    }
}
