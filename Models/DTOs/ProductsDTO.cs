using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EasyGamesWeb.Models.DTOs
{
    public class ProductsDTO
    {
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string ProductName { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }

        [Display(Name = "Description")]
        [MaxLength(300)]
        public string? ProductDescription { get; set; }

        
        [Display(Name = "Price (Legacy)")]
        [Range(0, 1_000_000)]
        public decimal Price { get; set; }

       
        [MaxLength(120)]
        public string? Source { get; set; }

        [Display(Name = "Buy Price")]
        [Range(0, 1_000_000)]
        public decimal BuyPrice { get; set; }

        [Display(Name = "Sell Price")]
        [Range(0, 1_000_000)]
        public decimal SellPrice { get; set; }

        [MaxLength(40)]
        public string? Sku { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        
        public string? Image { get; set; }
        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }
    }
}
