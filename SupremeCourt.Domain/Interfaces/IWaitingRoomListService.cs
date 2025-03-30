using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomListService
    {
        Task<WaitingRoom?> CreateWaitingRoomAsync(int gameId);
        Task<bool> JoinWaitingRoomAsync(int gameId, int playerId);
        Task<List<WaitingRoom>> GetAllWaitingRoomsAsync(); // ✅ Přidáno
        Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(); // ✅ Přidáno
    }
}