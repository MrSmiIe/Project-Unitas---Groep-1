namespace Project_Unitas.Models
{
    /// <summary>A bakery machine sold in the webshop.</summary>
    public class Machine
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Category { get; init; }

        /// <summary>One-line description for cards and lists.</summary>
        public required string Summary { get; init; }

        /// <summary>Longer description for the product page.</summary>
        public required string Description { get; init; }

        /// <summary>Price in euros, excluding VAT.</summary>
        public required decimal Price { get; init; }

        /// <summary>Technical specifications, shown in order (label → value).</summary>
        public IReadOnlyList<KeyValuePair<string, string>> Specs { get; init; } = [];
    }
}
