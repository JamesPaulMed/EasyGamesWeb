namespace EasyGamesWeb.Constants;

public enum UserTier { Bronze, Silver, Gold, Platinum }

public record TierThresholds(decimal BronzeMin, decimal SilverMin, decimal GoldMin, decimal PlatinumMin)
{
    public static TierThresholds Default => new(0, 750, 2000, 5000);
}

public record UserTierResult(UserTier Tier, decimal Profit, decimal NextTierAt, decimal RemainingToNext);

public static class Tiering
{
    public static UserTierResult FromProfit(decimal profit, TierThresholds t)
    {
        UserTier tier; decimal nextAt;
        if (profit >= t.PlatinumMin) { tier = UserTier.Platinum; nextAt = t.PlatinumMin; }
        else if (profit >= t.GoldMin) { tier = UserTier.Gold; nextAt = t.PlatinumMin; }
        else if (profit >= t.SilverMin) { tier = UserTier.Silver; nextAt = t.GoldMin; }
        else { tier = UserTier.Bronze; nextAt = t.SilverMin; }
        return new UserTierResult(tier, profit, nextAt, Math.Max(0, nextAt - profit));
    }
}
