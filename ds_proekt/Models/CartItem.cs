namespace ds_proekt.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string PerfumeId { get; set; }
        public int Quantity { get; set; }

        public Parfume Parfume { get; set; }
    }
}
