using Microsoft.EntityFrameworkCore;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly GameDbContext _context;

        public GameRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<Game?> GetByIdAsync(int id)
        {
            return await _context.Games.Include(g => g.Players).FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }
        public async Task AddAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task<Game?> GetActiveGameByPlayerIdAsync(int playerId) // Implementace
        {
            return await _context.Games
                .Include(g => g.Players)
                .FirstOrDefaultAsync(g => g.IsActive && g.Players.Any(p => p.Id == playerId));
        }


    }
}