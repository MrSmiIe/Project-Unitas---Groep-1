using System.ComponentModel.DataAnnotations;

namespace Project_Unitas.Models
{
    /// <summary>A bakery machine sold in the webshop (table "Machines").</summary>
    public class Machine
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(50)]
        public string Category { get; set; } = "";

        /// <summary>One-line description for cards and lists.</summary>
        [MaxLength(200)]
        public string Summary { get; set; } = "";

        /// <summary>Longer description for the product page.</summary>
        [MaxLength(2000)]
        public string Description { get; set; } = "";

        /// <summary>Price in euros, excluding VAT.</summary>
        public decimal Price { get; set; }

        /// <summary>Technical specifications, shown in SortOrder.</summary>
        public List<MachineSpec> Specs { get; set; } = [];
    }

    /// <summary>One line of a machine's specifications, e.g. "Vermogen: 7,5 kW" (table "MachineSpecs").</summary>
    public class MachineSpec
    {
        public int Id { get; set; }

        public int MachineId { get; set; }

        [MaxLength(60)]
        public string Label { get; set; } = "";

        [MaxLength(100)]
        public string Value { get; set; } = "";

        public int SortOrder { get; set; }
    }
}
