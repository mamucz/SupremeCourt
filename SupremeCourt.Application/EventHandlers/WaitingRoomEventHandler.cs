using SupremeCourt.Domain.Interfaces;

using SupremeCourt.Application.Services;

namespace SupremeCourt.Application.EventHandlers
{
    public class WaitingRoomEventHandler : IWaitingRoomEventHandler
    {
        private readonly IWaitingRoomNotifier _notifier;
        private readonly Lazy<WaitingRoomSessionManager> _sessionManager;

        public WaitingRoomEventHandler(
            IWaitingRoomNotifier notifier,
            Lazy<WaitingRoomSessionManager> sessionManager)
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
            _sessionManager.Value.RemoveSession(roomId);
            await _notifier.NotifyRoomExpiredAsync(roomId);
        }
    }
}