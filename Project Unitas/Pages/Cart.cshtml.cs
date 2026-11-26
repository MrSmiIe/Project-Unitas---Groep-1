using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Services;

namespace Project_Unitas.Pages
{
    public class CartModel(CartService cart, MachineCatalog catalog) : PageModel
    {
        public CartService Cart => cart;

        public void OnGet()
        {
        }

        public IActionResult OnPostAdd(int id, int quantity = 1)
        {
            var machine = catalog.Find(id);
            if (machine is null || quantity < 1)
            {
                return BadRequest();
            }

            cart.Add(id, quantity);
            TempData["StatusMessage"] = $"{machine.Name} is toegevoegd aan je winkelwagen.";

            // Go back to the page the visitor came from, if it is a page on this site.
            var referer = Request.Headers.Referer.ToString();
            if (Uri.TryCreate(referer, UriKind.Absolute, out var uri) &&
                uri.Host == Request.Host.Host &&
                Url.IsLocalUrl(uri.PathAndQuery))
            {
                return LocalRedirect(uri.PathAndQuery);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostUpdate(int id, int quantity)
        {
            // An empty or invalid number leaves the cart unchanged instead of removing the item.
            if (ModelState.IsValid)
            {
                cart.Update(id, quantity);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemove(int id)
        {
            var machine = catalog.Find(id);
            cart.Remove(id);
            if (machine is not null)
            {
                TempData["StatusMessage"] = $"{machine.Name} is verwijderd uit je winkelwagen.";
                TempData["StatusType"] = "info";
            }
            return RedirectToPage();
        }
    }
}
