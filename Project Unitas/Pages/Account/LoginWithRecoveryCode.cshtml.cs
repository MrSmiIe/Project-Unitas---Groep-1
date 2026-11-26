using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;

namespace Project_Unitas.Pages.Account
{
    /// <summary>Second login step when the phone is lost: one of the one-time recovery codes.</summary>
    public class LoginWithRecoveryCodeModel(
        SignInManager<ApplicationUser> signInManager,
        ILogger<LoginWithRecoveryCodeModel> logger) : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Vul een herstelcode in.")]
        [Display(Name = "Herstelcode")]
        public string RecoveryCode { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await signInManager.GetTwoFactorAuthenticationUserAsync() is null)
            {
                return RedirectToPage("/Account/Login");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user is null)
            {
                return RedirectToPage("/Account/Login");
            }

            var code = RecoveryCode.Replace(" ", string.Empty);
            var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(code);

            if (result.Succeeded)
            {
                logger.LogWarning("User {UserId} logged in with a recovery code.", user.Id);
                TempData["StatusMessage"] = "U bent ingelogd met een herstelcode. Elke code werkt maar één keer.";
                TempData["StatusType"] = "info";
                return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : Url.Content("~/Account"));
            }

            if (result.IsLockedOut)
            {
                logger.LogWarning("User {UserId} locked out after failed recovery codes.", user.Id);
                ModelState.AddModelError(string.Empty,
                    "Dit account is tijdelijk geblokkeerd na te veel mislukte pogingen. Probeer het over 15 minuten opnieuw.");
                return Page();
            }

            logger.LogWarning("Invalid recovery code for user {UserId}.", user.Id);
            ModelState.AddModelError(string.Empty, "Deze herstelcode is ongeldig.");
            return Page();
        }
    }
}
