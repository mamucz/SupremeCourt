using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Application.EventHandlers
{
    public class WaitingRoomEventHandler : IWaitingRoomEventHandler
    {
        private readonly IWaitingRoomNotifier _notifier;
        private readonly Lazy<IWaitingRoomSessionManager> _sessionManager;
        public WaitingRoomEventHandler(
            IWaitingRoomNotifier notifier,
            Lazy<IWaitingRoomSessionManager> sessionManager)
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

        public async Task NotifyPlayerJoinedAsync(int roomId, PlayerDto player, CancellationToken cancellationToken)
        {
            var session = _sessionManager.Value.GetSession(roomId);
            if (session == null)
                return;

            var roomDto = new WaitingRoomDto
            {
                WaitingRoomId = session.WaitingRoomId,
                CreatedAt = session.CreatedAt,
                CreatedByPlayerId = session.CreatedBy.Id,
                TimeLeftSeconds = session.GetTimeLeft(),
                Players = session.Players.Select(p => new PlayerDto
                {
                    PlayerId = p.Id,
                    Username = p.Username,
                    
                }).ToList()
            };

            await _notifier.NotifyRoomUpdatedAsync(roomDto);
        }
    }
}