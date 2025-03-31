using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomNotifier
    {
        Task NotifyPlayerJoinedAsync(int waitingRoomId, string playerName);
        Task NotifyWaitingRoomCreatedAsync(object dto); // již existuje
        Task NotifyCountdownTickAsync(int roomId, int secondsLeft); // 🕒
        Task NotifyRoomExpiredAsync(int roomId); // ⛔
        Task NotifyRoomUpdatedAsync(WaitingRoomDto dto); // ✅ PŘIDAT TUTO METODU
    }
}
