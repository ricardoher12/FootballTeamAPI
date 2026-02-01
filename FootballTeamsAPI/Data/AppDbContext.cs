using FootballTeamsAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace FootballTeamsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

        public DbSet<FootBallTeam> FootBallTeam => Set<FootBallTeam>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var dateOnlyConverter = new ValueConverter<DateOnly, string>(
                d => d.ToString("yyyy-MM-dd"),
                d => DateOnly.ParseExact(d, "yyyy-MM-dd")
            );

            modelBuilder.Entity<FootBallTeam>()
                .Property(e => e.DateOfLastWin)
                .HasConversion(dateOnlyConverter);
        }

    }
}
