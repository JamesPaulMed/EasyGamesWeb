using EasyGamesWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Admin")]
public class EmailsController : Controller
{
    private readonly IGroupEmailService _svc;
    public EmailsController(IGroupEmailService svc) => _svc = svc;

    public IActionResult Compose() => View(new EmailBlast());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Compose(EmailBlast a)
    {
        if (!ModelState.IsValid) return View(a);

        var count = await _svc.SendAsync(new EmailRequest(
            a.Segment, a.Subject, a.BodyHtml));

        TempData["Msg"] = $"Email sent to {count} recipients.";
        return RedirectToAction(nameof(Compose));
    }
}