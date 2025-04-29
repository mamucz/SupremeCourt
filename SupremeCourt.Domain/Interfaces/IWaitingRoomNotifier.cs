using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomNotifier
    {
        Task NotifyPlayerJoinedAsync(Guid waitingRoomId, PlayerDto player);
        Task NotifyWaitingRoomCreatedAsync(object dto); // již existuje
        Task NotifyCountdownTickAsync(Guid roomId, int secondsLeft); // 🕒
        Task NotifyRoomExpiredAsync(Guid roomId); // ⛔
        Task NotifyRoomUpdatedAsync(WaitingRoomDto dto); // ✅ PŘIDAT TUTO METODU
    }
}
