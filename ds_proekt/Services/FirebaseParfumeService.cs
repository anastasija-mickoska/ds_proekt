using ds_proekt.Models;
using Google.Cloud.Firestore;
using System.Threading.Tasks;

namespace ds_proekt.Services
{
    public class FirebaseParfumeService
    {
        private readonly FirestoreDb _firestoreDb;

        public FirebaseParfumeService()
        {
            _firestoreDb = FirestoreDb.Create("ds-proekt-baa0c"); 
        }

        public async Task AddParfumeAsync(Parfume parfume)
        {
            CollectionReference parfumesRef = _firestoreDb.Collection("Parfumes");
            await parfumesRef.AddAsync(parfume);
        }

        public async Task<List<Parfume>> GetParfumesAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("Parfumes").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Parfume>()).ToList();
        }

        public async Task<Parfume> GetParfumeByIdAsync(string id)
        {
            DocumentReference docRef = _firestoreDb.Collection("Parfumes").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<Parfume>() : null;
        }
        public async Task AddBrandAsync(Brand brand)
        {
            await _firestoreDb.Collection("Brands").AddAsync(brand);
        }

        public async Task<List<Brand>> GetBrandsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("Brands").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Brand>()).ToList();
        }
        public async Task AddReviewAsync(Review review)
        {
            await _firestoreDb.Collection("Reviews").AddAsync(review);
        }

        public async Task<List<Review>> GetReviewsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("Reviews").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Review>()).ToList();
        }

        public async Task<List<Review>> GetReviewsByProductAsync(string parfumeId)
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("Reviews").WhereEqualTo("ParfumeId", parfumeId).GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Review>()).ToList();
        }
        public async Task AddOrderAsync(Order order)
        {
            await _firestoreDb.Collection("Orders").AddAsync(order);
        }
        public async Task UpdateOrderAsync(Order order)
        {
            if (string.IsNullOrEmpty(order.Id))
                throw new ArgumentException("Order ID must not be null or empty.");

            DocumentReference docRef = _firestoreDb.Collection("Orders").Document(order.Id);

            await docRef.SetAsync(order);
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("Orders").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<Order>()).ToList();
        }

        public async Task<Order> GetOrderByUserIdAsync(string userId)
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("Orders").WhereEqualTo("UserId", userId).WhereEqualTo("OrderDate", null).Limit(1).GetSnapshotAsync();
            return snapshot.Documents.FirstOrDefault()?.ConvertTo<Order>();
        }
        public async Task AddToCartAsync(CartItem item)
        {
            await _firestoreDb.Collection("CartItems").AddAsync(item);
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("CartItems").GetSnapshotAsync();
            return snapshot.Documents.Select(d => d.ConvertTo<CartItem>()).ToList();
        }

        public async Task DeleteCartItemAsync(int id)
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("CartItems").WhereEqualTo("Id", id).Limit(1).GetSnapshotAsync();
            var doc = snapshot.Documents.FirstOrDefault();
            if (doc != null)
            {
                await doc.Reference.DeleteAsync();
            }
        }
    }
}
