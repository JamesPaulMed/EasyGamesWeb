namespace EasyGamesWeb.Models
{
    public class ShopSale
    {
        public int Id { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; } = null!;
        public int? CustomerId { get; set; }          // null if guest purchase
        public Customer? Customer { get; set; }
        public DateTime CreateDateUtc { get; set; } = DateTime.UtcNow;

        public string PaymentMethod { get; set; } = "Card"; // “NO CASH” assumption
        public bool IsPaid { get; set; } = true;            // POS is immediate pay
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        public ICollection<ShopSaleLine> Lines { get; set; } = new List<ShopSaleLine>();
    }
}
