using Microsoft.EntityFrameworkCore;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Interfaces;

namespace SupremeCourt.Infrastructure.Repositories
{
    public class WaitingRoomRepository : IWaitingRoomRepository
    {
        private readonly GameDbContext _context;

        public WaitingRoomRepository(GameDbContext context)
        {
            _context = context;
        }

        public async Task<WaitingRoom?> GetByIdAsync(int waitingRoomId)
        {
            return await _context.WaitingRooms
                .Include(w => w.Players)
                .FirstOrDefaultAsync(w => w.Id == waitingRoomId);
        }

        public async Task AddAsync(WaitingRoom waitingRoom)
        {
            _context.WaitingRooms.Add(waitingRoom);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WaitingRoom waitingRoom)
        {
            _context.WaitingRooms.Update(waitingRoom);
            await _context.SaveChangesAsync();
        }
        public async Task<List<WaitingRoom>> GetAllAsync() // ✅ Přidáno
        {
            return await _context.WaitingRooms.Include(w => w.Players).ToListAsync();
        }

        public async Task DeleteAsync(WaitingRoom waitingRoom)
        {
            _context.WaitingRooms.Remove(waitingRoom);
            await _context.SaveChangesAsync();
        }

        public async Task<WaitingRoom?> GetRoomByPlayerIdAsync(int playerId)
        {
            return await _context.WaitingRooms
                .Include(w => w.Players)
                .FirstOrDefaultAsync(w => w.Players.Any(p => p.Id == playerId));
        }

    }
}