using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace ds_proekt.Models
{
    [FirestoreData]
    public class User
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        [FirestoreProperty]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [FirestoreProperty]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [FirestoreProperty]
        public string Password { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [FirestoreProperty]
        public string Role { get; set; }
    }
}
