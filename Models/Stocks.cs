using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models
{

    [Table("stocks")]
    public class Stocks
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        public int Quantity { get; set; }  

        public Product? Product { get; set; } 


    }
}
