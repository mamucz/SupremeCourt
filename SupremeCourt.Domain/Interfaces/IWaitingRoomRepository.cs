using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomRepository
    {
        Task<WaitingRoom?> GetByIdAsync(int waitingRoomId);
        Task AddAsync(WaitingRoom waitingRoom);
        Task UpdateAsync(WaitingRoom waitingRoom);
        Task<List<WaitingRoom>> GetAllAsync(); // ✅ Přidáno
        Task DeleteAsync(WaitingRoom waitingRoom);

        Task<WaitingRoom?> GetRoomByPlayerIdAsync(int playerId);
    }
}