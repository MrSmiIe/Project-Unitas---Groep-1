using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Project_Unitas.Pages
{
    public record FaqItem(string Question, string Answer);

    public record FaqGroup(string Title, IReadOnlyList<FaqItem> Items);

    public class FAQModel : PageModel
    {
        // TODO: check these answers with the team (delivery times, warranty, etc.).
        public IReadOnlyList<FaqGroup> Groups { get; } =
        [
            new("Bestellen", [
                new("Hoe bestel ik een machine?",
                    "Voeg de machine toe aan uw winkelwagen en log in met uw klantaccount. Daarna kunt u de bestelling afronden. Heeft u nog geen account? Dan kunt u zich gratis registreren."),
                new("Zijn de prijzen inclusief btw?",
                    "Nee, alle prijzen in de webshop zijn exclusief 21% btw. In de winkelwagen ziet u het totaalbedrag inclusief btw."),
                new("Kan ik een offerte aanvragen?",
                    "Ja. Kies op de contactpagina het onderwerp “Offerte aanvragen” en vermeld welke machines u zoekt. We sturen u dan een offerte op maat."),
            ]),
            new("Levering en installatie", [
                new("Wordt mijn machine geïnstalleerd?",
                    "Ja. Onze technische buitendienst levert de machine af, installeert hem en legt uit hoe hij werkt."),
                new("Wat is de levertijd?",
                    "De levertijd verschilt per machine. U ontvangt een verwachte leverdatum bij het bevestigen van uw bestelling."),
            ]),
            new("Service en storingen", [
                new("Mijn machine heeft een storing. Wat nu?",
                    "Stuur ons een bericht via de contactpagina met het onderwerp “Storing of onderhoud”. Onze technische buitendienst neemt zo snel mogelijk contact met u op."),
                new("Bieden jullie onderhoud aan?",
                    "Ja, onze buitendienst voert periodiek onderhoud uit, zodat uw machines betrouwbaar blijven werken."),
            ]),
            new("Account en beveiliging", [
                new("Waarom moet ik tweestapsverificatie (MFA) gebruiken?",
                    "Met MFA heeft iemand naast uw wachtwoord ook een code van uw telefoon nodig om in te loggen. Zo blijft uw account veilig, zelfs als uw wachtwoord uitlekt."),
                new("Aan welke eisen moet mijn wachtwoord voldoen?",
                    "Minimaal 12 tekens, met ten minste één hoofdletter, één kleine letter, één cijfer en één speciaal teken (zoals ! of #)."),
                new("Wat kan ik in het klantenportaal?",
                    "In het klantenportaal ziet u uw bestellingen, de machines die u bij ons heeft gekocht en de status van uw serviceverzoeken."),
            ]),
        ];

        public void OnGet()
        {
        }
    }
}
