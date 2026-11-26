using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;

namespace Project_Unitas.Pages.Account
{
    /// <summary>Logging out only works with a POST (button), so a link on another site can't log you out.</summary>
    public class LogoutModel(SignInManager<ApplicationUser> signInManager) : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Account/Index");

        public async Task<IActionResult> OnPostAsync()
        {
            await signInManager.SignOutAsync();
            TempData["StatusMessage"] = "U bent uitgelogd.";
            TempData["StatusType"] = "info";
            return RedirectToPage("/Index");
        }
    }
}
