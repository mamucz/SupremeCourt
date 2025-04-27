using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;
using SupremeCourt.Domain.Sessions;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomListService
    {
        Task<WaitingRoomSession?> CreateWaitingRoomAsync(int createdByPlayerId, CancellationToken cancellationToken);
        Task<bool> JoinWaitingRoomAsync(int waitingRoomId, int playerId, CancellationToken cancellationToken);
        Task<List<WaitingRoomSession>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken);
        Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken);
    }
}