using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ds_proekt.Models
{
    [FirestoreData]
    public class Brand
    {
        [FirestoreProperty]
        public string BrandId { get; set; }
        [Required]
        [FirestoreProperty]
        public string Name { get; set; }

    }
}
