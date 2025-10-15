using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesWeb.Models
{
    public class Product
    {
        public int Id { get; set; }

        
        [Required, MaxLength(120)]
        public string ProductName { get; set; } = string.Empty;

        
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        [MaxLength(300)]
        public string? ShortDesc { get; set; }

        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1_000_000)]
        public decimal Price { get; set; }

        [MaxLength(200)]
        public string? Image { get; set; }

       
        [MaxLength(120)]
        public string? Source { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1_000_000)]
        public decimal BuyPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 1_000_000)]
        public decimal SellPrice { get; set; }

        [MaxLength(40)]
        public string? Sku { get; set; }

        public bool IsActive { get; set; } = true;

       
        [NotMapped]
        public decimal CostPrice
        {
            get => BuyPrice;
            set => BuyPrice = value;
        }

        
        [NotMapped]
        public int Quantity { get; set; }

       
        [NotMapped]
        public string? CategoryName
        {
            get => Category?.CategoryName ?? _categoryNameBacking;
            set => _categoryNameBacking = value;
        }
        private string? _categoryNameBacking;

       
        public ICollection<Stocks> Stocks { get; set; } = new List<Stocks>();

        
        [NotMapped] public decimal UnitProfit => SellPrice > 0 ? (SellPrice - BuyPrice) : 0m;
        [NotMapped] public decimal? MarginPercent => SellPrice > 0 ? (SellPrice - BuyPrice) / SellPrice * 100m : null;
    }
}
