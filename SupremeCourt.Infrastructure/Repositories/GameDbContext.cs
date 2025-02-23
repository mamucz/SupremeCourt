using Microsoft.EntityFrameworkCore;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Infrastructure
{
    public class GameDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameRound> GameRounds { get; set; }
        public DbSet<User> Users { get; set; }
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Players)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Game>()
                .HasMany(g => g.Rounds)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}