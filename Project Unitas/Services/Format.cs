using System.Globalization;

namespace Project_Unitas.Services
{
    /// <summary>Dutch number formatting, independent of the server's language settings.</summary>
    public static class Format
    {
        private static readonly NumberFormatInfo Dutch = new()
        {
            NumberDecimalSeparator = ",",
            NumberGroupSeparator = ".",
        };

        /// <summary>Formats a price as "€ 18.950,00".</summary>
        public static string Price(decimal amount) => "€ " + amount.ToString("N2", Dutch);
    }
}
