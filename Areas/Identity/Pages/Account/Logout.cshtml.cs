using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GestionFormations.Models;

namespace GestionFormations.Areas.Identity.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<Apprenant> _signInManager;

    public LogoutModel(SignInManager<Apprenant> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        await _signInManager.SignOutAsync();
        return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToPage("/Account/Login", new { area = "Identity" });
    }
}
