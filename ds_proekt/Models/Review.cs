using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ds_proekt.Models
{
    [FirestoreData]
    public class Review
    {
        [FirestoreProperty]
        public int ReviewId { get; set; }
        [FirestoreProperty]
        public int ParfumeId { get; set; }
        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        [StringLength(500)]
        public string Comment { get; set; }
        [FirestoreProperty]
        public int? Rating { get; set; }
    }
}
