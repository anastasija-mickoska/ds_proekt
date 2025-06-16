using ds_proekt.Models;
using Google.Cloud.Firestore;
using System.Threading.Tasks;
using Order = ds_proekt.Models.Order;

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
            var collection = _firestoreDb.Collection("Parfumes");

            DocumentReference docRef = await collection.AddAsync(parfume);
            parfume.ParfumeId = docRef.Id;
            Dictionary<string, object> update = new Dictionary<string, object>
            {
                { "ParfumeId", parfume.ParfumeId }
            };
            await docRef.UpdateAsync(update);
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
            DocumentReference docRef = _firestoreDb.Collection("orders").Document(order.Id); // Set doc ID manually
            await docRef.SetAsync(order);
        }
        public async Task UpdateOrderAsync(Order order)
        {
            var orderDoc = _firestoreDb.Collection("orders").Document(order.Id);

            var updateData = new Dictionary<string, object>
            {
                { "UserId", order.UserId },
                { "TotalPrice", order.TotalPrice },
                { "OrderDate", order.OrderDate },
                { "IsActive", order.IsActive },
                { "Items", order.Items.Select(i => new Dictionary<string, object>
                    {
                        { "ParfumeId", i.ParfumeId },
                        { "ParfumeName", i.ParfumeName },
                        { "Quantity", i.Quantity },
                        { "Price", i.Price }
                    }).ToList()
                }
            };

            await orderDoc.SetAsync(updateData, SetOptions.Overwrite);
        }


        public async Task<List<Order>> GetOrdersAsync()
        {
            var snapshot = await _firestoreDb.Collection("orders").GetSnapshotAsync();
            var orders = new List<Order>();

            foreach (var doc in snapshot.Documents)
            {
                var order = doc.ConvertTo<Order>();
                order.Id = doc.Id; 
                orders.Add(order);
            }

            return orders;
        }
        public async Task<Order> GetOrderByUserIdAsync(string userId)
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection("orders").WhereEqualTo("UserId", userId).WhereEqualTo("IsActive", true).Limit(1).GetSnapshotAsync();

            var doc = snapshot.Documents.FirstOrDefault();
            if (doc == null)
                return null;

            var dict = doc.ToDictionary();
            var order = new Order
            {
                Id = doc.Id,
                UserId = dict.ContainsKey("UserId") ? dict["UserId"]?.ToString() : null,
                TotalPrice = dict.ContainsKey("TotalPrice") && dict["TotalPrice"] != null ? Convert.ToDouble(dict["TotalPrice"]) : 0,
                OrderDate = dict.ContainsKey("OrderDate") && dict["OrderDate"] != null ? (DateTime?)Convert.ToDateTime(dict["OrderDate"]) : null,
                IsActive = dict.ContainsKey("IsActive") && dict["IsActive"] != null && Convert.ToBoolean(dict["IsActive"]),
                Items = new List<CartItem>()
            };


            if (dict.ContainsKey("Items") && dict["Items"] is IEnumerable<object> items)
            {
                foreach (var itemObj in items)
                {
                    if (itemObj is Dictionary<string, object> itemDict)
                    {
                        var cartItem = new CartItem
                        {
                            ParfumeId = itemDict.ContainsKey("ParfumeId") ? itemDict["ParfumeId"].ToString() : null,
                            ParfumeName = itemDict.ContainsKey("ParfumeName") ? itemDict["ParfumeName"].ToString() : null,
                            Quantity = itemDict.ContainsKey("Quantity") ? Convert.ToInt32(itemDict["Quantity"]) : 0,
                            Price = itemDict.ContainsKey("Price") ? Convert.ToDouble(itemDict["Price"]) : 0
                        };
                        order.Items.Add(cartItem);
                    }
                }
            }
            return order;
        }

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            var docRef = _firestoreDb.Collection("Orders").Document(orderId);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var order = snapshot.ConvertTo<Order>();
                order.Id = snapshot.Id; 
                return order;
            }

            return null;
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
        public async Task<List<User>> GetUsersAsync()
        {
            var snapshot = await _firestoreDb.Collection("users").GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<User>()).ToList();
        }
        public async Task<User?> GetUserByIdAsync(string userId)
        {
            var docRef = _firestoreDb.Collection("users").Document(userId);
            var snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                var user = snapshot.ConvertTo<User>();
                return user;
            }

            return null;
        }
    }
}
