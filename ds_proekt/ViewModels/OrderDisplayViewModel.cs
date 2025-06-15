using ds_proekt.Models;

namespace ds_proekt.ViewModels
{
    public class OrderDisplayViewModel
    {
        public string OrderId { get; set; }
        public string UserEmail { get; set; }
        public List<CartItem> Items { get; set; }
        public DateTime? OrderDate { get; set; }
        public double TotalPrice { get; set; }
    }
}
