using System.ComponentModel.DataAnnotations;

namespace EasyGamesWeb.Models
{
    public class OwnerStock
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
