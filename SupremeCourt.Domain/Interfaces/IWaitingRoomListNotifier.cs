using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Domain.Interfaces
{
    public interface IWaitingRoomListNotifier
    {
        Task NotifyWaitingRoomCreatedAsync(WaitingRoomDto roomDto);
    }
}