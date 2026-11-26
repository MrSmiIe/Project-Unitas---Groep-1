using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;

namespace Project_Unitas.Pages.Account
{
    public class LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger) : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; } = new();

        /// <summary>Page to return to after logging in (only local URLs are accepted).</summary>
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public IActionResult OnGet()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await signInManager.PasswordSignInAsync(
                Input.Email.Trim(), Input.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.RequiresTwoFactor)
            {
                // Password was right; now the authenticator code is needed.
                return RedirectToPage("/Account/LoginWith2fa", new { ReturnUrl });
            }

            if (result.Succeeded)
            {
                // Only happens for accounts that haven't set up MFA yet: send them there first.
                logger.LogInformation("User {Email} logged in without MFA; redirecting to MFA setup.", Input.Email);
                TempData["StatusMessage"] = "Stel eerst tweestapsverificatie in om uw account te beveiligen.";
                TempData["StatusType"] = "info";
                return RedirectToPage("/Account/Mfa");
            }

            if (result.IsLockedOut)
            {
                logger.LogWarning("Login for {Email} refused: account locked out. IP {Ip}.",
                    Input.Email, HttpContext.Connection.RemoteIpAddress);
                ModelState.AddModelError(string.Empty,
                    "Dit account is tijdelijk geblokkeerd na te veel mislukte pogingen. Probeer het over 15 minuten opnieuw.");
                return Page();
            }

            // Same message for "unknown e-mail" and "wrong password": don't reveal which accounts exist.
            logger.LogWarning("Failed login for {Email} from IP {Ip}.", Input.Email, HttpContext.Connection.RemoteIpAddress);
            ModelState.AddModelError(string.Empty, "Onjuist e-mailadres of wachtwoord.");
            return Page();
        }

        public class LoginInput
        {
            [Required(ErrorMessage = "Vul uw e-mailadres in.")]
            [EmailAddress(ErrorMessage = "Vul een geldig e-mailadres in.")]
            [Display(Name = "E-mailadres")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Vul uw wachtwoord in.")]
            [DataType(DataType.Password)]
            [Display(Name = "Wachtwoord")]
            public string Password { get; set; } = "";
        }
    }
}
