namespace EasyGamesWeb.Repositories;
using System.Net;
using System.Net.Mail;

public class SmtpEmailSender : IMarketingEmailSender
{
    private readonly IConfiguration _cfg;
    public SmtpEmailSender(IConfiguration cfg) => _cfg = cfg;

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var host = _cfg["Smtp:Host"]!;
        var port = int.TryParse(_cfg["Smtp:Port"], out var p) ? p : 587;
        var enable = bool.TryParse(_cfg["Smtp:Ssl"], out var e) ? e : true;
        var user = _cfg["Smtp:User"];
        var pass = _cfg["Smtp:Pass"];
        var from = _cfg["Smtp:FromEmail"]!;
        var fromName = _cfg["Smtp:FromName"];

        using var msg = new MailMessage
        {
            From = new MailAddress(from, string.IsNullOrWhiteSpace(fromName) ? from : fromName),
            Subject = subject ?? "",
            Body = htmlBody ?? "",
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress(toEmail));

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enable,
            UseDefaultCredentials = false,                 
            Credentials = new NetworkCredential(user, pass) 
        };

        await client.SendMailAsync(msg);
    }
}
