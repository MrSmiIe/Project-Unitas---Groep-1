using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;

namespace Project_Unitas.Pages.Account
{
    /// <summary>"Mijn account": the start of the customer portal (klantenportaal).</summary>
    public class IndexModel(UserManager<ApplicationUser> userManager) : PageModel
    {
        public ApplicationUser Customer { get; private set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            // MFA is required: accounts without it are sent to the setup first.
            if (!await userManager.GetTwoFactorEnabledAsync(user))
            {
                TempData["StatusMessage"] = "Stel eerst tweestapsverificatie in om uw account te gebruiken.";
                TempData["StatusType"] = "info";
                return RedirectToPage("/Account/Mfa");
            }

            Customer = user;
            return Page();
        }
    }
}
