using Microsoft.AspNetCore.Mvc.RazorPages;
using Project_Unitas.Models;
using Project_Unitas.Services;

namespace Project_Unitas.Pages
{
    public enum ServerState
    {
        Online,
        Offline,
        Onderhoud
    }

    public record ServerStatus(string Name, ServerState State, int? PingMs);

    public class IndexModel(MachineCatalog catalog) : PageModel
    {
        public IReadOnlyList<Machine> FeaturedMachines { get; private set; } = [];

        public IReadOnlyList<ServerStatus> Servers { get; private set; } = [];

        public int CountByState(ServerState state) => Servers.Count(s => s.State == state);

        /// <summary>Average ping of the servers that respond, or null when none do.</summary>
        public int? AveragePingMs
        {
            get
            {
                var pings = Servers.Where(s => s.PingMs.HasValue).Select(s => s.PingMs!.Value).ToList();
                return pings.Count == 0 ? null : (int)Math.Round(pings.Average());
            }
        }

        public async Task OnGetAsync()
        {
            FeaturedMachines = await catalog.GetAllAsync();

            // TODO: replace with real server status data.
            Servers =
            [
                new ServerStatus("Server 1", ServerState.Online, 24),
                new ServerStatus("Server 2", ServerState.Offline, null),
                new ServerStatus("Server 3", ServerState.Onderhoud, 180),
            ];
        }
    }
}
