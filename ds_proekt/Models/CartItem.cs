namespace ds_proekt.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ParfumeId { get; set; }
        public string ParfumeName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
