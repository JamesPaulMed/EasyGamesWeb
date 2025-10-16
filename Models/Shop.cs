namespace EasyGamesWeb.Models
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Location { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ShopUser> ShopUsers { get; set; } = new List<ShopUser>();
        public ICollection<ShopInventory> Inventory { get; set; } = new List<ShopInventory>();
    }

}
