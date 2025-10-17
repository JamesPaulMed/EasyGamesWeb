namespace EasyGamesWeb.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

public record EmailRequest(
    EmailSegment Segment,
    string Subject,
    string HtmlBody);

public interface IGroupEmailService
{
    Task<int> SendAsync(EmailRequest req);
}


public class GroupEmailService : IGroupEmailService
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _um;
    private readonly IMarketingEmailSender _sender;
    private readonly IUserSalesService _userSales; 
    private const int BatchSize = 50;

    public GroupEmailService(
        ApplicationDbContext db,
        UserManager<IdentityUser> um,
        IMarketingEmailSender sender,
        IUserSalesService userSales)
    {
        _db = db; _um = um; _sender = sender; _userSales = userSales;
    }

    private async Task<List<IdentityUser>> GetBaseEligibleUsersAsync(IQueryable<IdentityUser> baseQuery)
    {
        // LEFT JOIN EmailPreferences; include if no row OR AllowMarketing = true
        var q =
            from u in baseQuery
            join p in _db.EmailPreferences on u.Id equals p.UserId into gp
            from p in gp.DefaultIfEmpty()
            where (p == null || p.AllowMarketing)            // ← key line
                  && !string.IsNullOrEmpty(u.Email)          // has an email
            select u;

        return await q.AsNoTracking().ToListAsync();
    }




    public async Task<int> SendAsync(EmailRequest req)
    {
        var usersQuery = _um.Users.AsNoTracking();

        var marketing = _db.EmailPreferences.AsNoTracking()
                          .Where(p => p.AllowMarketing)
                          .Select(p => p.UserId);

        usersQuery = usersQuery.Where(u => marketing.Contains(u.Id));

        List<IdentityUser> list;

        switch (req.Segment)
        {
            case EmailSegment.AllUsers:
                list = await usersQuery.ToListAsync();
                break;

            case EmailSegment.Role_Admin:
                list = await FilterByRoleAsync(usersQuery, "Admin");
                break;

            case EmailSegment.Role_ShopOwner:
                list = await FilterByRoleAsync(usersQuery, "ShopOwner");
                break;

            case EmailSegment.Tier_Bronze:
            case EmailSegment.Tier_Silver:
            case EmailSegment.Tier_Gold:
            case EmailSegment.Tier_Platinum:
                list = await FilterByTierAsync(usersQuery, Map(req.Segment));
                break;

            default:
                list = new();
                break;
        }

        // send in batches
        int sent = 0;
        foreach (var chunk in list.Chunk(BatchSize))
        {
            foreach (var u in chunk)
            {
                if (string.IsNullOrWhiteSpace(u.Email)) continue;

             
                var html = req.HtmlBody.Replace("{{email}}", WebUtility.HtmlEncode(u.Email));
                await _sender.SendAsync(u.Email!, req.Subject, html);
                sent++;
            }
        }

        return sent;
    }

    private async Task<List<IdentityUser>> FilterByRoleAsync(IQueryable<IdentityUser> baseQuery, string role)
    {
        var roleIds = await _db.Roles.Where(r => r.Name == role).Select(r => r.Id).ToListAsync();
        var userIds = _db.UserRoles.Where(ur => roleIds.Contains(ur.RoleId)).Select(ur => ur.UserId);
        return await baseQuery.Where(u => userIds.Contains(u.Id)).ToListAsync();
    }

    private static UserTier Map(EmailSegment s) => s switch
    {
        EmailSegment.Tier_Bronze => UserTier.Bronze,
        EmailSegment.Tier_Silver => UserTier.Silver,
        EmailSegment.Tier_Gold => UserTier.Gold,
        EmailSegment.Tier_Platinum => UserTier.Platinum,
        _ => UserTier.Bronze
    };


    private async Task<List<IdentityUser>> FilterByTierAsync(IQueryable<IdentityUser> baseQuery, UserTier tier)
    {
        var users = await baseQuery.ToListAsync();

        var filtered = new List<IdentityUser>();
        foreach (var u in users)
        {
            var t = await _userSales.GetUserTier(u.Id);
            if (t.Tier == tier) filtered.Add(u);
        }
        return filtered;
    }


    
}
