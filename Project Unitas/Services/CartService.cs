using System.Text.Json;
using Project_Unitas.Models;

namespace Project_Unitas.Services
{
    public record CartLine(Machine Machine, int Quantity)
    {
        public decimal Total => Machine.Price * Quantity;
    }

    public record CartSummary(IReadOnlyList<CartLine> Lines)
    {
        public decimal Subtotal => Lines.Sum(l => l.Total);
        public decimal Vat => Math.Round(Subtotal * CartService.VatRate, 2);
        public decimal Total => Subtotal + Vat;
    }

    /// <summary>
    /// Shopping cart stored in the visitor's session (server-side; the browser only
    /// holds a session cookie). Only machine ids and quantities are stored — prices
    /// always come from the database, so they cannot be tampered with.
    /// </summary>
    public class CartService(IHttpContextAccessor httpContextAccessor, MachineCatalog catalog)
    {
        public const int MaxQuantity = 99;
        public const decimal VatRate = 0.21m;
        private const string SessionKey = "Cart";

        private ISession Session =>
            httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("Cart is only available during a request.");

        private Dictionary<int, int> Load()
        {
            var json = Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json)) return [];

            try
            {
                return JsonSerializer.Deserialize<Dictionary<int, int>>(json) ?? [];
            }
            catch (JsonException)
            {
                return [];
            }
        }

        private void Save(Dictionary<int, int> items) =>
            Session.SetString(SessionKey, JsonSerializer.Serialize(items));

        /// <summary>Number of items (no database needed; used for the header badge).</summary>
        public int Count => Load().Values.Sum();

        public async Task<CartSummary> GetSummaryAsync()
        {
            var items = Load();
            if (items.Count == 0) return new CartSummary([]);

            var machines = await catalog.FindManyAsync(items.Keys);
            var lines = machines
                .Select(m => new CartLine(m, items[m.Id]))
                .OrderBy(l => l.Machine.Id)
                .ToList();
            return new CartSummary(lines);
        }

        /// <returns>false when the machine does not exist.</returns>
        public async Task<bool> AddAsync(int machineId, int quantity = 1)
        {
            if (!await catalog.ExistsAsync(machineId)) return false;

            var items = Load();
            items[machineId] = Math.Clamp(items.GetValueOrDefault(machineId) + quantity, 1, MaxQuantity);
            Save(items);
            return true;
        }

        public void Update(int machineId, int quantity)
        {
            var items = Load();
            if (!items.ContainsKey(machineId)) return;

            if (quantity <= 0)
            {
                items.Remove(machineId);
            }
            else
            {
                items[machineId] = Math.Min(quantity, MaxQuantity);
            }

            Save(items);
        }

        public void Remove(int machineId) => Update(machineId, 0);
    }
}
