using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;


namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomListService
    {
        Task<WaitingRoomSession?> CreateWaitingRoomAsync(int createdByPlayerId, CancellationToken cancellationToken);
        Task<bool> JoinWaitingRoomAsync(Guid waitingRoomId, int playerId, CancellationToken cancellationToken);
        Task<List<WaitingRoomSession>> GetAllWaitingRoomsAsync(CancellationToken cancellationToken);
        Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync(CancellationToken cancellationToken);
    }
}