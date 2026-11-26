using System.Text.Json;
using Project_Unitas.Models;

namespace Project_Unitas.Services
{
    public record CartLine(Machine Machine, int Quantity)
    {
        public decimal Total => Machine.Price * Quantity;
    }

    /// <summary>
    /// Shopping cart stored in the visitor's session (server-side; the browser only
    /// holds a session cookie). Only machine ids and quantities are stored — prices
    /// always come from the catalog, so they cannot be tampered with.
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

        public IReadOnlyList<CartLine> Lines =>
            Load()
                .Select(item => (Machine: catalog.Find(item.Key), Quantity: item.Value))
                .Where(item => item.Machine is not null)
                .Select(item => new CartLine(item.Machine!, item.Quantity))
                .ToList();

        public int Count => Load().Values.Sum();

        public decimal Subtotal => Lines.Sum(l => l.Total);

        public decimal Vat => Math.Round(Subtotal * VatRate, 2);

        public decimal Total => Subtotal + Vat;

        /// <returns>false when the machine does not exist.</returns>
        public bool Add(int machineId, int quantity = 1)
        {
            if (catalog.Find(machineId) is null) return false;

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
