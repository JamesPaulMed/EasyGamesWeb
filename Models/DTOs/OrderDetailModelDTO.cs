namespace EasyGamesWeb.Models.DTOs
{
    public class OrderDetailModelDTO
    {
        public string DivId { get; set; }
        public IEnumerable<OrderDetails> OrderDetail { get; set; }
    }
}
