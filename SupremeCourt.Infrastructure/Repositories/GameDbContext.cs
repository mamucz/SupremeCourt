using Microsoft.EntityFrameworkCore;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Infrastructure
{
    public class GameDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; } // Přidáno
        public DbSet<GameRound> GameRounds { get; set; } // Přidáno
    
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1:M vztah mezi Game a Players
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Players)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);  // ⚠️ Zabránění cyklům smazání

            // 1:M vztah mezi Game a GameRounds
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Rounds)
                .WithOne(r => r.Game)
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Restrict);  // ⚠️ Zabránění cyklům smazání

            modelBuilder.Entity<Player>()
                .HasOne(p => p.User)
                .WithOne(u => u.Player)
                .HasForeignKey<Player>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}