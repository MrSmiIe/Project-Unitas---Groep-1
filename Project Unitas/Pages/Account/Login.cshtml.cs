using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Unitas.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: check the credentials against the user database, then ask for the MFA code.
            // Security: give the same message for "unknown e-mail" and "wrong password",
            // and lock the account for a while after too many failed attempts.

            TempData["StatusMessage"] = "Inloggen is nog niet beschikbaar: de koppeling met de klantendatabase en MFA wordt nog gebouwd.";
            TempData["StatusType"] = "info";
            return RedirectToPage();
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
