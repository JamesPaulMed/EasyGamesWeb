using Microsoft.AspNetCore.Identity;

namespace EasyGamesWeb.Models
{
    public class ShopUser
    {
        public int ShopId { get; set; }
        public Shop Shop { get; set; } = null!;
        public string UserId { get; set; } = "";
        public IdentityUser User { get; set; } = null!;
        public bool IsManager { get; set; } = false;
    }
}
