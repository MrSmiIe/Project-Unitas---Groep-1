using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;

namespace Project_Unitas.Pages.Account
{
    /// <summary>Second login step: the 6-digit code from the authenticator app.</summary>
    public class LoginWith2faModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginWith2faModel> logger) : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "Vul de code uit uw authenticator-app in.")]
        [StringLength(7, MinimumLength = 6, ErrorMessage = "De code bestaat uit 6 cijfers.")]
        [Display(Name = "Code uit de app")]
        public string Code { get; set; } = "";

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Only reachable right after a correct password.
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

            var code = Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent: false, rememberClient: false);

            if (result.Succeeded)
            {
                logger.LogInformation("User {UserId} logged in with MFA.", user.Id);
                return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : Url.Content("~/Account"));
            }

            if (result.IsLockedOut)
            {
                logger.LogWarning("User {UserId} locked out after failed MFA codes.", user.Id);
                ModelState.AddModelError(string.Empty,
                    "Dit account is tijdelijk geblokkeerd na te veel mislukte pogingen. Probeer het over 15 minuten opnieuw.");
                return Page();
            }

            logger.LogWarning("Invalid MFA code for user {UserId}.", user.Id);
            ModelState.AddModelError(string.Empty, "Deze code is onjuist of verlopen.");
            return Page();
        }
    }
}
