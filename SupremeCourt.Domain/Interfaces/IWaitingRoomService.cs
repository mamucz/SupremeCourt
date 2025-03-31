using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomService
    {
        /// <summary>
        /// Creates a new waiting room and starts countdown.
        /// </summary>
        /// <param name="createdByPlayerId">ID of the player creating the room.</param>
        /// <returns>The created waiting room as entity (can be null if player is not found).</returns>
        Task<WaitingRoom?> CreateWaitingRoomAsync(int createdByPlayerId);

        /// <summary>
        /// Joins a player to a waiting room if they are not already in another one.
        /// </summary>
        /// <param name="waitingRoomId">ID of the waiting room.</param>
        /// <param name="playerId">ID of the player joining.</param>
        /// <returns>True if joined successfully, otherwise false.</returns>
        Task<bool> JoinWaitingRoomAsync(int waitingRoomId, int playerId);

        /// <summary>
        /// Gets all waiting rooms as entities (internal use only).
        /// </summary>
        Task<List<WaitingRoom>> GetAllWaitingRoomsAsync();

        /// <summary>
        /// Gets summary info (DTOs) of all active waiting rooms.
        /// </summary>
        Task<List<WaitingRoomDto>> GetWaitingRoomSummariesAsync();

        /// <summary>
        /// Gets detailed DTO representation of a waiting room by ID.
        /// Includes player list and remaining time.
        /// </summary>
        Task<WaitingRoomDto?> GetWaitingRoomByIdAsync(int roomId);
    }
}
