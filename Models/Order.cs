using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models

{
    [Table("Order")]
    public class Order
    {

        public int Id { set; get; }
        [Required]

        public string UserId { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        [Required]

        public int OrderStatId { get; set; }

        public bool IsDeleted { get; set; } = false;

        [Required]
        [MaxLength(20)]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(60)]
        public string? Email { get; set; }

        [Required]
        public string? MobileNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string? Address { get; set; }
        [Required]
        [MaxLength (20)]

        public string? PaymentMethod { get; set; }

        public bool isPaid { get; set; }

        public OrderStat OrderStat { get; set; }

        public List<OrderDetails> OrderDetail { get; set; }
    }
}