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

        public async Task<WaitingRoom?> GetByIdAsync(int waitingRoomId, CancellationToken cancellationToken)
        {
            var waitingroom = await _context.WaitingRooms
                .Include(w => w.Players)
                .ThenInclude(p => p.User) // ✅ TADY přidej
                .FirstOrDefaultAsync(w => w.Id == waitingRoomId);
            return waitingroom;
        }

        public async Task AddAsync(WaitingRoom waitingRoom, CancellationToken cancellationToken)
        {
            _context.WaitingRooms.Add(waitingRoom);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WaitingRoom waitingRoom, CancellationToken cancellationToken)
        {
            _context.WaitingRooms.Update(waitingRoom);
            await _context.SaveChangesAsync();
        }
        public async Task<List<WaitingRoom>> GetAllAsync(CancellationToken cancellationToken) // ✅ Přidáno
        {
            return await _context.WaitingRooms.Include(w => w.Players).ToListAsync();
        }

        public async Task DeleteAsync(WaitingRoom waitingRoom, CancellationToken cancellationToken)
        {
            _context.WaitingRooms.Remove(waitingRoom);
            await _context.SaveChangesAsync();
        }

        public async Task<WaitingRoom?> GetRoomByPlayerIdAsync(int playerId, CancellationToken cancellationToken)
        {
            return await _context.WaitingRooms
                .Include(w => w.Players)
                .FirstOrDefaultAsync(w => w.Players.Any(p => p.Id == playerId));
        }

    }
}