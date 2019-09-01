namespace Warehouse.Models
{
    public class StockItem
    {
        //the catalogId
        public string Id { get; set; }
        
        public int Quantity { get; set; }

        public int MaxAllowedStock { get; set; }
        public int MinimumAllowedStock { get; set; }
    }
}