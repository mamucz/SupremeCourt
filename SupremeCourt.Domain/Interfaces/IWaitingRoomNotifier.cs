namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomNotifier
    {
        Task NotifyPlayerJoinedAsync(int waitingRoomId, string playerName);
        Task NotifyWaitingRoomCreatedAsync(object dto); // už máš
        Task NotifyCountdownTickAsync(int roomId, int secondsLeft); // 🕒
        Task NotifyRoomExpiredAsync(int roomId); // ⛔
    }
}
