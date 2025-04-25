using Microsoft.EntityFrameworkCore;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Infrastructure.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly GameDbContext _context;

        public PlayerRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Player?> GetByIdAsync(int id)
        {
            return await _context.Players
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Player?> GetByUserIdAsync(int userId)
        {
            return await _context.Players
                .Include(p => p.User)
                .Where(p => !p.User.Deleted) // Ignorujeme hráče, jejichž User je smazán
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }


        public async Task AddAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Player player)
        {
            _context.Players.Remove(player);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Player>> GetAllAsync()
        {
            return await _context.Players.ToListAsync();
        }

        public async Task UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Player>> GetAllAiPlayersAsync(CancellationToken cancellationToken)
        {
            return await _context.Players
                .Include(p => p.User)
                .Where(p => p.IsAi)
                .ToListAsync(cancellationToken);
        }

    }
}