using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models
{

    [Table("OrderStat")]
    public class OrderStat
    {

        public int Id { get; set; }

        [Required]

        public int StatusId { get; set; }
        [Required, MaxLength(20)]

        public string ?StatName { get; set; }

        public bool IsFinal { get; set; } = false;
    }
}
