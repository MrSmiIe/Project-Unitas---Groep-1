using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;
using QRCoder;

namespace Project_Unitas.Pages.Account
{
    /// <summary>
    /// Sets up two-factor authentication (MFA) with an authenticator app
    /// (Microsoft Authenticator, Google Authenticator, ...). MFA is required for every
    /// customer account, so there is deliberately no "turn MFA off" button.
    /// </summary>
    public class MfaModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<MfaModel> logger) : PageModel
    {
        private const string Issuer = "Unitas Bakery Systems";

        public bool IsEnabled { get; private set; }

        /// <summary>The secret key, grouped per 4 characters for typing it in by hand.</summary>
        public string SharedKey { get; private set; } = "";

        /// <summary>QR code (PNG as data URI) containing the otpauth:// link for the app.</summary>
        public string QrCodeDataUri { get; private set; } = "";

        /// <summary>Shown once, right after enabling MFA.</summary>
        [TempData]
        public string? NewRecoveryCodes { get; set; }

        public int RecoveryCodesLeft { get; private set; }

        [BindProperty]
        [Required(ErrorMessage = "Vul de code uit uw authenticator-app in.")]
        [StringLength(7, MinimumLength = 6, ErrorMessage = "De code bestaat uit 6 cijfers.")]
        [Display(Name = "Code uit de app")]
        public string Code { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return Challenge();

            if (await userManager.GetTwoFactorEnabledAsync(user))
            {
                return RedirectToPage();
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var code = Code.Replace(" ", string.Empty).Replace("-", string.Empty);
            var valid = await userManager.VerifyTwoFactorTokenAsync(
                user, userManager.Options.Tokens.AuthenticatorTokenProvider, code);

            if (!valid)
            {
                ModelState.AddModelError(nameof(Code), "Deze code klopt niet. Controleer of de tijd op uw telefoon goed staat en probeer het opnieuw.");
                await LoadAsync(user);
                return Page();
            }

            await userManager.SetTwoFactorEnabledAsync(user, true);
            var recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            NewRecoveryCodes = string.Join(" ", recoveryCodes ?? []);

            // Refresh the login cookie so it reflects the new security stamp.
            await signInManager.RefreshSignInAsync(user);

            logger.LogInformation("User {UserId} enabled MFA.", user.Id);
            TempData["StatusMessage"] = "Tweestapsverificatie staat aan. Bewaar uw herstelcodes op een veilige plek.";
            return RedirectToPage();
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            IsEnabled = await userManager.GetTwoFactorEnabledAsync(user);

            if (IsEnabled)
            {
                RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);
                return;
            }

            var key = await userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await userManager.ResetAuthenticatorKeyAsync(user);
                key = await userManager.GetAuthenticatorKeyAsync(user) ?? "";
            }

            SharedKey = FormatKey(key);

            var email = await userManager.GetEmailAsync(user) ?? "";
            var otpauthUri =
                $"otpauth://totp/{Uri.EscapeDataString(Issuer)}:{Uri.EscapeDataString(email)}" +
                $"?secret={key}&issuer={Uri.EscapeDataString(Issuer)}&digits=6";

            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(otpauthUri, QRCodeGenerator.ECCLevel.Q);
            var png = new PngByteQRCode(data).GetGraphic(6);
            QrCodeDataUri = "data:image/png;base64," + Convert.ToBase64String(png);
        }

        private static string FormatKey(string key)
        {
            var result = new StringBuilder();
            for (var i = 0; i < key.Length; i += 4)
            {
                if (i > 0) result.Append(' ');
                result.Append(key.AsSpan(i, Math.Min(4, key.Length - i)));
            }
            return result.ToString().ToLowerInvariant();
        }
    }
}
