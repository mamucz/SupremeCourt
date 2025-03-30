using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Application.Sessions;

namespace SupremeCourt.Application.EventHandlers
{
    public class WaitingRoomEventHandler : IWaitingRoomEventHandler
    {
        private readonly IWaitingRoomNotifier _notifier;
        private readonly WaitingRoomSessionManager _sessionManager;

        public WaitingRoomEventHandler(
            IWaitingRoomNotifier notifier,
            WaitingRoomSessionManager sessionManager)
        {
            _notifier = notifier;
            _sessionManager = sessionManager;
        }

        public async Task HandleCountdownTickAsync(int roomId, int secondsLeft)
        {
            await _notifier.NotifyCountdownTickAsync(roomId, secondsLeft);
        }

        public async Task HandleRoomExpiredAsync(int roomId)
        {
            _sessionManager.RemoveSession(roomId);
            await _notifier.NotifyRoomExpiredAsync(roomId);
        }
    }
}