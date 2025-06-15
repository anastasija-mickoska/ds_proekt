
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;
namespace ds_proekt.Models;
[FirestoreData]
public class Parfume
{
    [FirestoreProperty]
    public string ParfumeId { get; set; }
        [Required]
    [FirestoreProperty]
    public string Name { get; set; }
    [FirestoreProperty]
    [Required]
        public decimal Price { get; set; }
    [FirestoreProperty]
    public string? ImageUrl { get; set; }
    [FirestoreProperty]
    public int BrandId { get; set; }
    [FirestoreProperty]
    public Brand Brand { get; set; }
    [FirestoreProperty]
    public ICollection<Review>? Reviews { get; set; }

       
}

