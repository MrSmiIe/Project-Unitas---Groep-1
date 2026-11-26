using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;

namespace Project_Unitas.Pages.Account
{
    public class RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger) : PageModel
    {
        [BindProperty]
        public RegisterInput Input { get; set; } = new();

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

            var email = Input.Email.Trim();
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = Input.Name.Trim(),
                CompanyName = Input.Company.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
            };

            // Identity stores only a salted HASH of the password, never the password itself,
            // and checks it against the password policy in Program.cs.
            var result = await userManager.CreateAsync(user, Input.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            logger.LogInformation("New customer account created: {UserId}.", user.Id);

            await signInManager.SignInAsync(user, isPersistent: false);
            TempData["StatusMessage"] = "Uw account is aangemaakt. Stel nu tweestapsverificatie in.";
            return RedirectToPage("/Account/Mfa");
        }

        public class RegisterInput
        {
            [Required(ErrorMessage = "Vul de bedrijfsnaam in.")]
            [StringLength(100, ErrorMessage = "De bedrijfsnaam mag maximaal 100 tekens lang zijn.")]
            [Display(Name = "Bedrijfsnaam")]
            public string Company { get; set; } = "";

            [Required(ErrorMessage = "Vul uw naam in.")]
            [StringLength(100, ErrorMessage = "Uw naam mag maximaal 100 tekens lang zijn.")]
            [Display(Name = "Naam")]
            public string Name { get; set; } = "";

            [Required(ErrorMessage = "Vul uw e-mailadres in.")]
            [EmailAddress(ErrorMessage = "Vul een geldig e-mailadres in.")]
            [StringLength(254)]
            [Display(Name = "E-mailadres")]
            public string Email { get; set; } = "";

            // Strong password policy (also enforced by Identity in Program.cs).
            [Required(ErrorMessage = "Kies een wachtwoord.")]
            [StringLength(128, MinimumLength = 12, ErrorMessage = "Uw wachtwoord moet minimaal 12 tekens lang zijn.")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).*$",
                ErrorMessage = "Gebruik minimaal één hoofdletter, één kleine letter, één cijfer en één speciaal teken.")]
            [DataType(DataType.Password)]
            [Display(Name = "Wachtwoord")]
            public string Password { get; set; } = "";

            [Required(ErrorMessage = "Herhaal uw wachtwoord.")]
            [Compare(nameof(Password), ErrorMessage = "De wachtwoorden komen niet overeen.")]
            [DataType(DataType.Password)]
            [Display(Name = "Herhaal wachtwoord")]
            public string ConfirmPassword { get; set; } = "";
        }
    }
}
