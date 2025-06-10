
using System.ComponentModel.DataAnnotations;
namespace ds_proekt.Models;
public class Parfume
{
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public ICollection<Review>? Reviews { get; set; }
       
}

