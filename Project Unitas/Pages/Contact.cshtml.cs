using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Unitas.Pages
{
    public class ContactModel : PageModel
    {
        public static readonly string[] Subjects =
        [
            "Vraag over een machine",
            "Offerte aanvragen",
            "Storing of onderhoud",
            "Mijn bestelling",
            "Overig",
        ];

        [BindProperty]
        public ContactInput Input { get; set; } = new();

        public void OnGet(string? onderwerp)
        {
            if (onderwerp is not null && Subjects.Contains(onderwerp))
            {
                Input.Subject = onderwerp;
            }
        }

        public IActionResult OnPost()
        {
            // Only accept the subjects from the list (an empty one is already caught by [Required]).
            if (!string.IsNullOrEmpty(Input.Subject) && !Subjects.Contains(Input.Subject))
            {
                ModelState.AddModelError("Input.Subject", "Kies een onderwerp uit de lijst.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: store the message in the database or e-mail it to customer service.
            // Don't write the message or personal data to the logs (privacy).

            TempData["StatusMessage"] = "Bedankt voor uw bericht! We nemen zo snel mogelijk contact met u op.";
            return RedirectToPage();
        }

        public class ContactInput
        {
            [Required(ErrorMessage = "Vul uw naam in.")]
            [StringLength(100, ErrorMessage = "Uw naam mag maximaal 100 tekens lang zijn.")]
            [Display(Name = "Naam")]
            public string Name { get; set; } = "";

            [Display(Name = "Bedrijfsnaam (optioneel)")]
            [StringLength(100, ErrorMessage = "De bedrijfsnaam mag maximaal 100 tekens lang zijn.")]
            public string? Company { get; set; }

            [Required(ErrorMessage = "Vul uw e-mailadres in.")]
            [EmailAddress(ErrorMessage = "Vul een geldig e-mailadres in.")]
            [StringLength(254)]
            [Display(Name = "E-mailadres")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "Kies een onderwerp.")]
            [Display(Name = "Onderwerp")]
            public string Subject { get; set; } = "";

            [Required(ErrorMessage = "Schrijf een bericht.")]
            [StringLength(2000, MinimumLength = 10, ErrorMessage = "Uw bericht moet tussen de 10 en 2000 tekens lang zijn.")]
            [Display(Name = "Bericht")]
            public string Message { get; set; } = "";
        }
    }
}
