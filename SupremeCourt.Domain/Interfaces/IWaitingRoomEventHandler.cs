using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomEventHandler
    {
        Task HandleCountdownTickAsync(Guid roomId, int secondsLeft);
        Task HandleRoomExpiredAsync(Guid roomId);
        Task NotifyPlayerJoinedAsync(Guid roomId, PlayerDto player, CancellationToken cancellationToken);
    }
}