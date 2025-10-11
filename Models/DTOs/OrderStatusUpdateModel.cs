using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EasyGamesWeb.Models.DTOs
{
    public class OrderStatusUpdateModel
    {
        public int OrderId { get; set; }

        [Required]
         
        public int OrderStatId { get; set; }

        public IEnumerable<SelectListItem>? OrderStatList { get; set; }
    }
}
