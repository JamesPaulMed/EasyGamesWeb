namespace EasyGamesWeb.Models
{
    public class ShopSaleLine
    {
        public int Id { get; set; }
        public int ShopSaleId { get; set; }
        public ShopSale ShopSale { get; set; } = null!;
        public int ShopInventoryId { get; set; }
        public ShopInventory ShopInventory { get; set; } = null!;
        public int ProductId { get; set; }

        public decimal UnitPrice { get; set; }    
        public decimal UnitCost { get; set; }      
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }     
        public decimal LineProfit { get; set; }     
    }
}
