namespace EasyGamesWeb.Models.DTOs
{
    public class StocksDisplayModel
    {
        public int Id   { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string? ProductName { get; set; }
    }
}
