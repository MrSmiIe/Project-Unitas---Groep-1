using Project_Unitas.Models;

namespace Project_Unitas.Data
{
    /// <summary>
    /// Starting data for the Machines and MachineSpecs tables. It is part of the
    /// database migration, so it ends up in the database together with the tables.
    /// To change machines later, change them in the database (or add a new migration).
    /// </summary>
    internal static class MachineSeed
    {
        public static readonly Machine[] Machines =
        [
            new() { Id = 1, Name = "Spiraalkneder SK-80", Category = "Deegbereiding", Price = 18950m,
                Summary = "Krachtige spiraalkneder voor grote hoeveelheden deeg.",
                Description = "De SK-80 kneedt tot 80 kg deeg per keer met twee snelheden en een omkeerbare kuip. Geschikt voor brood-, pizza- en zoetdeeg." },
            new() { Id = 2, Name = "Deegverdeler DV-30", Category = "Deegverwerking", Price = 12400m,
                Summary = "Verdeelt deeg in gelijke stukken van 50 tot 1000 gram.",
                Description = "De DV-30 verdeelt een deegklomp in 30 gelijke stukken per cyclus. Het gewicht is traploos instelbaar." },
            new() { Id = 3, Name = "Opboller OB-2000", Category = "Deegverwerking", Price = 9750m,
                Summary = "Bolt tot 2000 deegstukken per uur rond op.",
                Description = "De OB-2000 maakt van verdeelde deegstukken mooie, gelijkmatige bollen. Ideaal in combinatie met de DV-30." },
            new() { Id = 4, Name = "Langmaker LM-600", Category = "Deegverwerking", Price = 8300m,
                Summary = "Rolt deegbollen uit tot langwerpige broden.",
                Description = "De LM-600 rolt en vormt deeg tot stokbroden, puntjes en tijgerbroden. De walsafstand is per product in te stellen." },
            new() { Id = 5, Name = "Rijskast RK-20", Category = "Rijzen", Price = 6900m,
                Summary = "Rijskast met regeling van temperatuur en luchtvochtigheid.",
                Description = "De RK-20 biedt plaats aan 20 bakplaten. Temperatuur en luchtvochtigheid worden automatisch op peil gehouden." },
            new() { Id = 6, Name = "Dekoven DO-4", Category = "Bakken", Price = 34500m,
                Summary = "Dekoven met vier stenen bakvloeren.",
                Description = "De DO-4 heeft vier onafhankelijk regelbare dekken met stenen vloer, voor een krokante korst. Inclusief stoominstallatie." },
            new() { Id = 7, Name = "Rekoven RO-18", Category = "Bakken", Price = 27800m,
                Summary = "Roterende rekoven voor een gelijkmatig bakresultaat.",
                Description = "In de RO-18 draait een volledig rek met 18 platen rond, zodat alles even bruin wordt. Gas- en elektrische uitvoering." },
            new() { Id = 8, Name = "Broodsnijmachine BS-11", Category = "Afwerking", Price = 5450m,
                Summary = "Snijdt broden in gelijke plakken van 11 mm.",
                Description = "De BS-11 snijdt tot 600 broden per uur. Veilig door een afgeschermd mes en noodstop." },
        ];

        private static readonly (int MachineId, string Label, string Value)[] SpecRows =
        [
            (1, "Capaciteit", "80 kg deeg"), (1, "Snelheden", "2 + omkeerbaar"), (1, "Vermogen", "7,5 kW"), (1, "Afmetingen (b × d × h)", "75 × 125 × 140 cm"),
            (2, "Stukken per cyclus", "30"), (2, "Gewicht per stuk", "50 – 1000 g"), (2, "Vermogen", "1,5 kW"), (2, "Afmetingen (b × d × h)", "60 × 70 × 130 cm"),
            (3, "Capaciteit", "2000 stuks per uur"), (3, "Gewicht per stuk", "30 – 1200 g"), (3, "Vermogen", "0,75 kW"), (3, "Afmetingen (b × d × h)", "65 × 65 × 120 cm"),
            (4, "Walsbreedte", "600 mm"), (4, "Capaciteit", "1500 stuks per uur"), (4, "Vermogen", "0,55 kW"), (4, "Afmetingen (b × d × h)", "90 × 80 × 125 cm"),
            (5, "Capaciteit", "20 platen (60 × 40 cm)"), (5, "Temperatuur", "20 – 45 °C"), (5, "Vermogen", "3 kW"), (5, "Afmetingen (b × d × h)", "80 × 95 × 205 cm"),
            (6, "Dekken", "4"), (6, "Bakoppervlak", "9,6 m²"), (6, "Max. temperatuur", "300 °C"), (6, "Vermogen", "36 kW"),
            (7, "Capaciteit", "18 platen (60 × 80 cm)"), (7, "Max. temperatuur", "280 °C"), (7, "Vermogen", "45 kW"), (7, "Afmetingen (b × d × h)", "140 × 200 × 240 cm"),
            (8, "Snijdikte", "11 mm"), (8, "Capaciteit", "600 broden per uur"), (8, "Vermogen", "0,37 kW"), (8, "Afmetingen (b × d × h)", "70 × 60 × 85 cm"),
        ];

        public static readonly MachineSpec[] Specs = SpecRows
            .Select((row, index) => new MachineSpec
            {
                Id = index + 1,
                MachineId = row.MachineId,
                Label = row.Label,
                Value = row.Value,
                SortOrder = SpecRows.Take(index).Count(r => r.MachineId == row.MachineId),
            })
            .ToArray();
    }
}
