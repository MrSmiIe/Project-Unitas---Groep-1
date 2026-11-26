using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Unitas.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: save the account in the database (store a password HASH, never the password
            // itself — ASP.NET Core Identity does this for you) and set up MFA.

            TempData["StatusMessage"] = "Registreren is nog niet beschikbaar: de koppeling met de klantendatabase wordt nog gebouwd.";
            TempData["StatusType"] = "info";
            return RedirectToPage();
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

            // Strong password policy (see the project's security requirements).
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
