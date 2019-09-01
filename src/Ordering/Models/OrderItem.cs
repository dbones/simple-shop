namespace Ordering.Models
{
    public class OrderItem
    {
        public string Id { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public bool ValidatedStock { get; set; }
    }
}