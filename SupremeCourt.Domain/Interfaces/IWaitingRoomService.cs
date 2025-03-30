using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomService
    {
        Task<WaitingRoom?> CreateWaitingRoomAsync(int createdByPlayerId);
        Task<bool> JoinWaitingRoomAsync(int waitingRoomId, int playerId);
        Task<List<WaitingRoom>> GetAllWaitingRoomsAsync();
        Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync();
        Task<WaitingRoomDto?> GetWaitingRoomByIdAsync(int roomId);
    }
}