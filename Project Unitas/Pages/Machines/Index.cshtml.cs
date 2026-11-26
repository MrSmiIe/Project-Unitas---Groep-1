using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;
using Project_Unitas.Services;

namespace Project_Unitas.Pages.Machines
{
    public class IndexModel(MachineCatalog catalog) : PageModel
    {
        /// <summary>Search term from the header search box (?zoek=...).</summary>
        [BindProperty(SupportsGet = true, Name = "zoek")]
        public string? Query { get; set; }

        /// <summary>Selected category (?categorie=...).</summary>
        [BindProperty(SupportsGet = true, Name = "categorie")]
        public string? Category { get; set; }

        public IReadOnlyList<Machine> Machines { get; private set; } = [];

        public IReadOnlyList<string> Categories { get; private set; } = [];

        public async Task OnGetAsync()
        {
            Machines = await catalog.SearchAsync(Query, Category);
            Categories = await catalog.GetCategoriesAsync();
        }
    }
}
