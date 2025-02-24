using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomRepository
    {
        Task<WaitingRoom?> GetByGameIdAsync(int gameId);
        Task AddAsync(WaitingRoom waitingRoom);
        Task UpdateAsync(WaitingRoom waitingRoom);
        Task<List<WaitingRoom>> GetAllAsync(); // ✅ Přidáno
    }
}