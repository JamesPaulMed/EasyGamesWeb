namespace EasyGamesWeb.Models
{
    public class ShopInventory
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public decimal UnitPrice { get; set; }        
        public decimal UnitCost { get; set; }       
        public int Quantity { get; set; }       
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    }
}
