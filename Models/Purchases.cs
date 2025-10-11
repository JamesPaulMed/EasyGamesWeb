namespace EasyGamesWeb.Models;

public record UserPurchaseRow(
    int OrderId,
    DateTime CreateDate,
    string ProductName,
    int Quantity,
    double UnitPrice,
    double LineTotal);

public class Purchases
{
    public int OrderId { get; set; }
    public DateTime CreateDate { get; set; }
    public List<UserPurchaseRow> Lines { get; set; } = new();
    public double OrderTotal { get; set; }
}
