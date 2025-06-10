using System.ComponentModel.DataAnnotations;

namespace ds_proekt.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int ParfumeId { get; set; }
        public string UserId { get; set; }
        [StringLength(500)]
        public string Comment { get; set; }
        public int? Rating { get; set; }
    }
}
