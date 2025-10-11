using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models
{

    [Table("CartDetails")]
    public class CartDetails
    {

        public int Id { get; set; }

        [Required]

        public int CartId { get; set; }
        [Required]

        public int ProductId { get; set; }
        [Required]

        public int Quantity { set; get; }

        [Required]
        public double UnitPrice { set; get; }

       public Product Products { get; set; }
       public Cart Carts { get; set; }




    }
}
