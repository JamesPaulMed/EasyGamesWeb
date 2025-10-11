using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models
{

    [Table("Product")]
    public class Product
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(30)]

        public string? ProductName { get; set; }

        public string? ShortDesc { get; set; }
        public double Price { get; set; }

        public double CostPrice { get; set; }
        public string? Image { get; set; }

        [Required]

        public int CategoryId { get; set; }

        public Category Categories { get; set; }

        public List<OrderDetails> OrderDetail { get; set; }

        public List<CartDetails> CartDetails { get; set; }

        public Stocks Stocks { get; set; }  

        [NotMapped]

        public string CategoryName { get; set; }
        public int Quantity { get; set; }

    }
}
