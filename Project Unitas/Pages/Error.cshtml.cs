using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Project_Unitas.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        /// <summary>HTTP status code (e.g. 404) when shown for a status code instead of a crash.</summary>
        public int? StatusCodeValue { get; set; }

        public bool IsNotFound => StatusCodeValue == StatusCodes.Status404NotFound;

        public void OnGet(int? code) => Load(code);

        // Errors on POST requests are re-executed as POST as well.
        public void OnPost(int? code) => Load(code);

        private void Load(int? code)
        {
            StatusCodeValue = code;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
