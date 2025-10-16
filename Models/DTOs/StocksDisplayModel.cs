namespace EasyGamesWeb.Models.DTOs
{
    public class StocksDisplayModel
    {
        public int Id   { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string? ProductName { get; set; }
        
        public string? Source { get; set; }

        public decimal BuyPrice { get; set; }

        public decimal SellPrice { get; set; }

        public decimal UnitProfit => SellPrice - BuyPrice;

        public decimal? MarginPercent => SellPrice > 0 ? (SellPrice - BuyPrice) / SellPrice * 100m : null;

        public decimal TotalProfit => UnitProfit * Quantity;
    }
}
