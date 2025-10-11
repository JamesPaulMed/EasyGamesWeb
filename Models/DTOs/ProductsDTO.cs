using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace EasyGamesWeb.Models.DTOs
{
    public class ProductsDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]

        public string? ProductName { get; set; }

        [Required]
        [MaxLength(30)]

        public string? ProductDescription { get; set; }

        [Required]

        public double Price { get; set; }

        public string? Image { get; set; }

        [Required]

        public int CategoryId { get; set; }

        public IFormFile? ImageFile { get; set; }   

        public IEnumerable<SelectListItem>? CategoryList { get; set;}
    }
}
