using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;
using Project_Unitas.Services;

namespace Project_Unitas.Pages.Machines
{
    public class DetailsModel(MachineCatalog catalog) : PageModel
    {
        public Machine Machine { get; private set; } = default!;

        public IActionResult OnGet(int id)
        {
            var machine = catalog.Find(id);
            if (machine is null) return NotFound();

            Machine = machine;
            return Page();
        }
    }
}
