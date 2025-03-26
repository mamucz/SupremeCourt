using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomService
    {
        Task<WaitingRoom?> CreateWaitingRoomAsync(int gameId);
        Task<bool> JoinWaitingRoomAsync(int gameId, int playerId);
        Task<bool> IsTimeExpiredAsync(int gameId);
        Task<List<WaitingRoom>> GetAllWaitingRoomsAsync(); // ✅ Přidáno
        Task<List<WaitingRoomInfoDto>> GetWaitingRoomSummariesAsync(); // ✅ Přidáno
    }
}