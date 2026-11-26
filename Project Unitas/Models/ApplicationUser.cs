using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Project_Unitas.Models
{
    /// <summary>
    /// A customer account. ASP.NET Core Identity provides e-mail, the password HASH,
    /// lockout and two-factor (MFA) fields; we add the customer's name and company.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; } = "";

        [MaxLength(100)]
        public string CompanyName { get; set; } = "";

        public DateTime CreatedAtUtc { get; set; }
    }
}
