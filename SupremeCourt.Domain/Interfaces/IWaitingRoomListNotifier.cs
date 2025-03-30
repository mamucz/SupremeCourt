namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomListNotifier
    {
        Task NotifyWaitingRoomCreatedAsync(object roomDto);
    }
}