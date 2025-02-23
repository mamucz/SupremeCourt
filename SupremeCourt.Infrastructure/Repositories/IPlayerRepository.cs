using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SupremeCourt.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly GameDbContext _context;

        public PlayerRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetByIdAsync(int id) =>
            await _context.Players.FindAsync(id);

        public async Task AddAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Player>> GetAllAsync() =>
            await _context.Players.ToListAsync();
    }
}