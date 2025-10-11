using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models
{

    [Table("OrderDetails")]
    public class OrderDetails
    {

        public int Id { get; set; }


        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public double UnitPrice { get; set; }

        public double UnitPriceAtSale { get; set; }
        public double UnitCostAtSale { get; set; }

        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
