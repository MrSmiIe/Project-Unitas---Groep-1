using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Project_Unitas.Models;

namespace Project_Unitas.Data
{
    /// <summary>
    /// The website's MySQL database. Besides our own tables it contains the
    /// ASP.NET Core Identity tables (AspNetUsers etc.) for customer accounts and MFA.
    /// </summary>
    public class UnitasDbContext(DbContextOptions<UnitasDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<MachineSpec> MachineSpecs => Set<MachineSpec>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Machine>(machine =>
            {
                machine.Property(m => m.Price).HasPrecision(10, 2);
                machine.HasIndex(m => m.Category);
                machine.HasMany(m => m.Specs)
                    .WithOne()
                    .HasForeignKey(s => s.MachineId)
                    .OnDelete(DeleteBehavior.Cascade);
                machine.HasData(MachineSeed.Machines);
            });

            builder.Entity<MachineSpec>().HasData(MachineSeed.Specs);

            builder.Entity<ContactMessage>().HasIndex(m => m.CreatedAtUtc);
        }
    }

    /// <summary>
    /// Used only by the "dotnet ef" tool to create migrations and SQL scripts.
    /// It never connects to a database, so it needs no real password.
    /// </summary>
    public class UnitasDbContextDesignTimeFactory : IDesignTimeDbContextFactory<UnitasDbContext>
    {
        public UnitasDbContext CreateDbContext(string[] args) =>
            new(new DbContextOptionsBuilder<UnitasDbContext>()
                .UseMySQL("Server=localhost;Database=unitas;User=design-time-only")
                .Options);
    }
}
