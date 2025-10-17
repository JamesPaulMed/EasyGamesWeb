namespace EasyGamesWeb.Repositories
{
    public interface IMarketingEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
    }
}
