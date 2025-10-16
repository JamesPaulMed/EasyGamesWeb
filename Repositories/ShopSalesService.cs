using Microsoft.EntityFrameworkCore;
using EasyGamesWeb.Models;

namespace EasyGamesWeb.Repositories
{
    public interface IShopSalesService
    {
        Task<(Customer customer, UserTier? tier)> FindOrCreateCustomerByPhone(string phone);
        Task<ShopSale> CreateSale(int shopId, int? customerId, IEnumerable<(int shopInventoryId, int qty)> items);
    }

    public class ShopSalesService : IShopSalesService
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserSalesService _userSales; // your existing tier service

        private static readonly Dictionary<UserTier, decimal> TierDiscounts = new()
        {
            [UserTier.Bronze] = 0.00m,
            [UserTier.Silver] = 0.05m,
            [UserTier.Gold] = 0.10m,
            [UserTier.Platinum] = 0.15m
        };

        public ShopSalesService(ApplicationDbContext db, IUserSalesService userSales)
        {
            _db = db;
            _userSales = userSales;
        }

        private static string NormalizePhone(string phone) =>
            new string((phone ?? "").Where(char.IsDigit).ToArray());

        /// <summary>
        /// Finds a customer by phone or creates a new one if not found.
        /// Returns the customer and their current tier (if linked to an Identity user).
        /// </summary>
        public async Task<(Customer customer, UserTier? tier)> FindOrCreateCustomerByPhone(string phone)
        {
            var norm = NormalizePhone(phone);
            if (string.IsNullOrWhiteSpace(norm))
                throw new InvalidOperationException("Phone number is required.");

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.PhoneNormalized == norm);

            if (customer == null)
            {
                customer = new Customer { PhoneNormalized = norm };
                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();
                return (customer, null); // new guest
            }

            UserTier? tier = null;
            if (!string.IsNullOrEmpty(customer.IdentityUserId))
            {
                var tierInfo = await _userSales.GetUserTier(customer.IdentityUserId);
                tier = tierInfo.Tier;
            }

            return (customer, tier);
        }

        /// Creates a shop sale (no cash).////
        /// Applies tier discounts if the customer has a linked user account. ///
        /// Reduces stock for each sold item (can go below zero). ///

        public async Task<ShopSale> CreateSale(int shopId, int? customerId, IEnumerable<(int shopInventoryId, int qty)> items)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            // compute discount if linked to a user with a tier
            decimal discountRate = 0m;
            if (customerId is int cid)
            {
                var cust = await _db.Customers.FindAsync(cid);
                if (!string.IsNullOrEmpty(cust?.IdentityUserId))
                {
                    var tierInfo = await _userSales.GetUserTier(cust.IdentityUserId);
                    discountRate = TierDiscounts[tierInfo.Tier];
                }
            }

            var sale = new ShopSale
            {
                ShopId = shopId,
                CustomerId = customerId,
                PaymentMethod = "Card",
                IsPaid = true,
                Lines = new List<ShopSaleLine>()
            };

            decimal subtotal = 0m;

            foreach (var (shopInvId, qty) in items)
            {
                if (qty <= 0) continue;

                var inv = await _db.ShopInventories
                    .Include(i => i.Product)
                    .FirstAsync(i => i.Id == shopInvId && i.ShopId == shopId);

                var lineTotal = inv.UnitPrice * qty;
                var lineProfit = (inv.UnitPrice - inv.UnitCost) * qty;

                sale.Lines.Add(new ShopSaleLine
                {
                    ShopInventoryId = inv.Id,
                    ProductId = inv.ProductId,
                    UnitPrice = inv.UnitPrice,
                    UnitCost = inv.UnitCost,
                    Quantity = qty,
                    LineTotal = lineTotal,
                    LineProfit = lineProfit
                });

                // Decrement shop stock (can go negative; just warn in UI)
                inv.Quantity -= qty;
                inv.LastUpdatedUtc = DateTime.UtcNow;

                subtotal += lineTotal;
            }

            sale.Subtotal = subtotal;
            sale.Discount = Math.Round(subtotal * discountRate, 2);
            sale.Total = sale.Subtotal - sale.Discount;

            _db.ShopSales.Add(sale);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return sale;
        }
    }
}
