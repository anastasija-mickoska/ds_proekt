using System.ComponentModel.DataAnnotations;

namespace ds_proekt.Models
{
    public class Brand
    {
        public int BrandId { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
