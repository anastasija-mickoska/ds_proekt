using ds_proekt.Models;
using Firebase.Database;
using Firebase.Database.Query;
using System.Threading.Tasks;

namespace ds_proekt.Services
{
    public class FirebaseParfumeService
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseParfumeService()
        {
            _firebaseClient = new FirebaseClient("https://ds-proekt-baa0c-default-rtdb.europe-west1.firebasedatabase.app/");
        }

        public async Task AddParfumeAsync(Parfume parfume)
        {
            await _firebaseClient
                .Child("Parfumes")
                .PostAsync(parfume);
        }
        public async Task<List<Parfume>> GetParfumesAsync()
        {
            return (await _firebaseClient
                .Child("Parfumes")
                .OnceAsync<Parfume>())
                .Select(item => item.Object)
                .ToList();
        }
        public async Task<Parfume> GetParfumeByIdAsync(int parfumeId)
        {
            var result = await _firebaseClient
                .Child("Parfumes") // Make sure you're querying the right node
                .OnceAsync<Parfume>();
            return result
                .FirstOrDefault(x => x.Object.ParfumeId == parfumeId)
                ?.Object;
        }
        public async Task AddBrandAsync(Brand brand)
        {
            await _firebaseClient
                .Child("Brands")
                .PostAsync(brand);
        }
        public async Task<List<Brand>> GetBrandsAsync()
        {
            return (await _firebaseClient
                .Child("Brands")
                .OnceAsync<Brand>())
                .Select(item => item.Object)
                .ToList();
        }
        public async Task AddReviewAsync(Review review)
        {
            await _firebaseClient
                .Child("Reviews")
                .PostAsync(review);
        }
        public async Task<List<Review>> GetReviewsAsync()
        {
            return (await _firebaseClient
                .Child("Reviews")
                .OnceAsync<Review>())
                .Select(item => item.Object)
                .ToList();
        }
        public async Task<List<Review>> GetReviewsByProductAsync(int parfumeId)
        {
            var allReviews = await _firebaseClient
                .Child("Reviews")
                .OnceAsync<Review>();

            var matchingReviews = allReviews
                .Where(x => x.Object.ParfumeId == parfumeId)
                .Select(x => x.Object)
                .ToList();

            return matchingReviews;
        }
        public async Task AddOrderAsync(Order order)
        {
            await _firebaseClient
                .Child("Orders")
                .PostAsync(order);
        }
        public async Task<List<Order>> GetOrdersAsync()
        {
            var orders = await _firebaseClient
                .Child("Orders")
                .OnceAsync<Order>();

            return orders.Select(o => o.Object).ToList();
        }
        public async Task AddToCartAsync(CartItem item)
        {
            await _firebaseClient
                .Child("CartItems")
                .PostAsync(item);
        }
        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            return (await _firebaseClient
                .Child("CartItems")
                .OnceAsync<CartItem>())
                .Select(item => item.Object)
                .ToList();
        }

        public async Task DeleteCartItemAsync(int key)
        {
            var toDelete = (await _firebaseClient
                .Child("CartItems")
                .OnceAsync<CartItem>())
                .FirstOrDefault(x => x.Object.Id == key);

            if (toDelete != null)
            {
                await _firebaseClient.Child("CartItems").Child(toDelete.Key).DeleteAsync();
            }
        }
    }
}
