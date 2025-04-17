using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomListService
    {
        Task<WaitingRoom?> CreateWaitingRoomAsync(int gameId, CancellationToken cancellationToken);
        Task<bool> JoinWaitingRoomAsync(int gameId, int playerId, CancellationToken cancellationToken);
        Task<List<WaitingRoom>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken); // ✅ Přidáno
        Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken); // ✅ Přidáno
    }
}