using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomEventHandler
    {
        Task HandleCountdownTickAsync(int roomId, int secondsLeft);
        Task HandleRoomExpiredAsync(int roomId);
        Task NotifyPlayerJoinedAsync(int roomId, PlayerDto player, CancellationToken cancellationToken);
    }
}