using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace EasyGamesWeb.Models.DTOs
{
    public class StocksDTO
    {

        public int ProductId { get; set; }

        [Range (0, int.MaxValue, ErrorMessage = "Quantity must not be negative")]
        public int Quantity { get; set; }
    }
}
    