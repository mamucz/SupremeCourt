using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomRepository
    {
        Task<WaitingRoom?> GetByIdAsync(int waitingRoomId, CancellationToken cancellationToken);
        Task AddAsync(WaitingRoom waitingRoom, CancellationToken cancellationToken);
        Task UpdateAsync(WaitingRoom waitingRoom, CancellationToken cancellationToken);
        Task<List<WaitingRoom>> GetAllAsync( CancellationToken cancellationToken); // ✅ Přidáno
        Task DeleteAsync(WaitingRoom waitingRoom, CancellationToken cancellationToken);
        Task<WaitingRoom?> GetRoomByPlayerIdAsync(int playerId, CancellationToken cancellationToken);
    }
}