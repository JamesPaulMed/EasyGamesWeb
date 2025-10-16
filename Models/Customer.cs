namespace EasyGamesWeb.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? IdentityUserId { get; set; } 
        public string PhoneNormalized { get; set; } = ""; 
        public string? Name { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
