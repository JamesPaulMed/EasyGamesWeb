namespace EasyGamesWeb.Models
{
    public class EmailPreference
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!; 
    public bool AllowMarketing { get; set; } = true;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
  
}
}
