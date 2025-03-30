namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomEventHandler
    {
        Task HandleCountdownTickAsync(int roomId, int secondsLeft);
        Task HandleRoomExpiredAsync(int roomId);
    }
}