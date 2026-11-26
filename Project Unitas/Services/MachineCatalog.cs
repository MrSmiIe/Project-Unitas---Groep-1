using Microsoft.EntityFrameworkCore;
using Project_Unitas.Data;
using Project_Unitas.Models;

namespace Project_Unitas.Services
{
    /// <summary>Reads the machines for the webshop from the database.</summary>
    public class MachineCatalog(UnitasDbContext db)
    {
        public Task<List<Machine>> GetAllAsync() =>
            db.Machines.AsNoTracking().OrderBy(m => m.Id).ToListAsync();

        /// <summary>Categories in the order of the baking process (= order of the first machine in each).</summary>
        public Task<List<string>> GetCategoriesAsync() =>
            db.Machines
                .GroupBy(m => m.Category)
                .Select(g => new { Category = g.Key, FirstId = g.Min(m => m.Id) })
                .OrderBy(c => c.FirstId)
                .Select(c => c.Category)
                .ToListAsync();

        /// <summary>One machine including its specifications, or null.</summary>
        public Task<Machine?> FindAsync(int id) =>
            db.Machines
                .AsNoTracking()
                .Include(m => m.Specs.OrderBy(s => s.SortOrder))
                .FirstOrDefaultAsync(m => m.Id == id);

        public Task<bool> ExistsAsync(int id) => db.Machines.AnyAsync(m => m.Id == id);

        public Task<List<Machine>> FindManyAsync(IEnumerable<int> ids)
        {
            var idList = ids.ToList();
            return db.Machines.AsNoTracking().Where(m => idList.Contains(m.Id)).ToListAsync();
        }

        /// <summary>
        /// Filters by category and/or a search term (name, category or summary).
        /// MySQL's default collation compares case-insensitively.
        /// </summary>
        public Task<List<Machine>> SearchAsync(string? query, string? category)
        {
            IQueryable<Machine> result = db.Machines.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(category))
            {
                result = result.Where(m => m.Category == category);
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var q = query.Trim();
                result = result.Where(m =>
                    m.Name.Contains(q) || m.Category.Contains(q) || m.Summary.Contains(q));
            }

            return result.OrderBy(m => m.Id).ToListAsync();
        }
    }
}
