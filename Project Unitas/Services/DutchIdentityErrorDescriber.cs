using Microsoft.AspNetCore.Identity;

namespace Project_Unitas.Services
{
    /// <summary>Dutch versions of the account system's error messages.</summary>
    public class DutchIdentityErrorDescriber : IdentityErrorDescriber
    {
        private static IdentityError Error(string code, string description) => new() { Code = code, Description = description };

        public override IdentityError DefaultError() => Error(nameof(DefaultError), "Er is iets misgegaan. Probeer het opnieuw.");
        public override IdentityError DuplicateEmail(string email) => Error(nameof(DuplicateEmail), "Er bestaat al een account met dit e-mailadres.");
        public override IdentityError DuplicateUserName(string userName) => Error(nameof(DuplicateUserName), "Er bestaat al een account met dit e-mailadres.");
        public override IdentityError InvalidEmail(string? email) => Error(nameof(InvalidEmail), "Dit e-mailadres is ongeldig.");
        public override IdentityError InvalidUserName(string? userName) => Error(nameof(InvalidUserName), "Dit e-mailadres is ongeldig.");
        public override IdentityError PasswordMismatch() => Error(nameof(PasswordMismatch), "Het wachtwoord is onjuist.");
        public override IdentityError PasswordTooShort(int length) => Error(nameof(PasswordTooShort), $"Het wachtwoord moet minimaal {length} tekens lang zijn.");
        public override IdentityError PasswordRequiresDigit() => Error(nameof(PasswordRequiresDigit), "Het wachtwoord moet minimaal één cijfer bevatten.");
        public override IdentityError PasswordRequiresLower() => Error(nameof(PasswordRequiresLower), "Het wachtwoord moet minimaal één kleine letter bevatten.");
        public override IdentityError PasswordRequiresUpper() => Error(nameof(PasswordRequiresUpper), "Het wachtwoord moet minimaal één hoofdletter bevatten.");
        public override IdentityError PasswordRequiresNonAlphanumeric() => Error(nameof(PasswordRequiresNonAlphanumeric), "Het wachtwoord moet minimaal één speciaal teken bevatten.");
        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => Error(nameof(PasswordRequiresUniqueChars), $"Het wachtwoord moet minimaal {uniqueChars} verschillende tekens bevatten.");
        public override IdentityError InvalidToken() => Error(nameof(InvalidToken), "De code is ongeldig of verlopen.");
        public override IdentityError RecoveryCodeRedemptionFailed() => Error(nameof(RecoveryCodeRedemptionFailed), "Deze herstelcode is ongeldig.");
        public override IdentityError ConcurrencyFailure() => Error(nameof(ConcurrencyFailure), "Uw gegevens zijn intussen gewijzigd. Probeer het opnieuw.");
        public override IdentityError UserLockoutNotEnabled() => Error(nameof(UserLockoutNotEnabled), "Blokkeren is niet ingeschakeld voor dit account.");
    }
}
