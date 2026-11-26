using Project_Unitas.Models;

namespace Project_Unitas.Services
{
    /// <summary>
    /// Provides the machines shown in the webshop.
    /// TODO: replace the hard-coded list with machines from the database.
    /// The rest of the site only talks to this class, so only this file has to change.
    /// </summary>
    public class MachineCatalog
    {
        private static List<KeyValuePair<string, string>> Spec(params (string Label, string Value)[] specs) =>
            specs.Select(s => KeyValuePair.Create(s.Label, s.Value)).ToList();

        private readonly IReadOnlyList<Machine> _machines =
        [
            new Machine
            {
                Id = 1,
                Name = "Spiraalkneder SK-80",
                Category = "Deegbereiding",
                Summary = "Krachtige spiraalkneder voor grote hoeveelheden deeg.",
                Description = "De SK-80 kneedt tot 80 kg deeg per keer met twee snelheden en een omkeerbare kuip. Geschikt voor brood-, pizza- en zoetdeeg.",
                Price = 18950m,
                Specs = [.. Spec(("Capaciteit", "80 kg deeg"), ("Snelheden", "2 + omkeerbaar"), ("Vermogen", "7,5 kW"), ("Afmetingen (b × d × h)", "75 × 125 × 140 cm"))],
            },
            new Machine
            {
                Id = 2,
                Name = "Deegverdeler DV-30",
                Category = "Deegverwerking",
                Summary = "Verdeelt deeg in gelijke stukken van 50 tot 1000 gram.",
                Description = "De DV-30 verdeelt een deegklomp in 30 gelijke stukken per cyclus. Het gewicht is traploos instelbaar.",
                Price = 12400m,
                Specs = [.. Spec(("Stukken per cyclus", "30"), ("Gewicht per stuk", "50 – 1000 g"), ("Vermogen", "1,5 kW"), ("Afmetingen (b × d × h)", "60 × 70 × 130 cm"))],
            },
            new Machine
            {
                Id = 3,
                Name = "Opboller OB-2000",
                Category = "Deegverwerking",
                Summary = "Bolt tot 2000 deegstukken per uur rond op.",
                Description = "De OB-2000 maakt van verdeelde deegstukken mooie, gelijkmatige bollen. Ideaal in combinatie met de DV-30.",
                Price = 9750m,
                Specs = [.. Spec(("Capaciteit", "2000 stuks per uur"), ("Gewicht per stuk", "30 – 1200 g"), ("Vermogen", "0,75 kW"), ("Afmetingen (b × d × h)", "65 × 65 × 120 cm"))],
            },
            new Machine
            {
                Id = 4,
                Name = "Langmaker LM-600",
                Category = "Deegverwerking",
                Summary = "Rolt deegbollen uit tot langwerpige broden.",
                Description = "De LM-600 rolt en vormt deeg tot stokbroden, puntjes en tijgerbroden. De walsafstand is per product in te stellen.",
                Price = 8300m,
                Specs = [.. Spec(("Walsbreedte", "600 mm"), ("Capaciteit", "1500 stuks per uur"), ("Vermogen", "0,55 kW"), ("Afmetingen (b × d × h)", "90 × 80 × 125 cm"))],
            },
            new Machine
            {
                Id = 5,
                Name = "Rijskast RK-20",
                Category = "Rijzen",
                Summary = "Rijskast met regeling van temperatuur en luchtvochtigheid.",
                Description = "De RK-20 biedt plaats aan 20 bakplaten. Temperatuur en luchtvochtigheid worden automatisch op peil gehouden.",
                Price = 6900m,
                Specs = [.. Spec(("Capaciteit", "20 platen (60 × 40 cm)"), ("Temperatuur", "20 – 45 °C"), ("Vermogen", "3 kW"), ("Afmetingen (b × d × h)", "80 × 95 × 205 cm"))],
            },
            new Machine
            {
                Id = 6,
                Name = "Dekoven DO-4",
                Category = "Bakken",
                Summary = "Dekoven met vier stenen bakvloeren.",
                Description = "De DO-4 heeft vier onafhankelijk regelbare dekken met stenen vloer, voor een krokante korst. Inclusief stoominstallatie.",
                Price = 34500m,
                Specs = [.. Spec(("Dekken", "4"), ("Bakoppervlak", "9,6 m²"), ("Max. temperatuur", "300 °C"), ("Vermogen", "36 kW"))],
            },
            new Machine
            {
                Id = 7,
                Name = "Rekoven RO-18",
                Category = "Bakken",
                Summary = "Roterende rekoven voor een gelijkmatig bakresultaat.",
                Description = "In de RO-18 draait een volledig rek met 18 platen rond, zodat alles even bruin wordt. Gas- en elektrische uitvoering.",
                Price = 27800m,
                Specs = [.. Spec(("Capaciteit", "18 platen (60 × 80 cm)"), ("Max. temperatuur", "280 °C"), ("Vermogen", "45 kW"), ("Afmetingen (b × d × h)", "140 × 200 × 240 cm"))],
            },
            new Machine
            {
                Id = 8,
                Name = "Broodsnijmachine BS-11",
                Category = "Afwerking",
                Summary = "Snijdt broden in gelijke plakken van 11 mm.",
                Description = "De BS-11 snijdt tot 600 broden per uur. Veilig door een afgeschermd mes en noodstop.",
                Price = 5450m,
                Specs = [.. Spec(("Snijdikte", "11 mm"), ("Capaciteit", "600 broden per uur"), ("Vermogen", "0,37 kW"), ("Afmetingen (b × d × h)", "70 × 60 × 85 cm"))],
            },
        ];

        public IReadOnlyList<Machine> All => _machines;

        public IEnumerable<string> Categories => _machines.Select(m => m.Category).Distinct();

        public Machine? Find(int id) => _machines.FirstOrDefault(m => m.Id == id);

        /// <summary>Filters by category and/or a search term (name, category or summary).</summary>
        public IReadOnlyList<Machine> Search(string? query, string? category)
        {
            IEnumerable<Machine> result = _machines;

            if (!string.IsNullOrWhiteSpace(category))
            {
                result = result.Where(m => m.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                result = result.Where(m =>
                    m.Name.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    m.Category.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    m.Summary.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            return result.ToList();
        }
    }
}
