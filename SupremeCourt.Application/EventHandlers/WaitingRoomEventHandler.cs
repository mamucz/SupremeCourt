using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Sessions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Application.EventHandlers
{
    public class WaitingRoomEventHandler : IWaitingRoomEventHandler
    {
        private readonly IWaitingRoomNotifier _notifier;
        private readonly IWaitingRoomSessionManager _sessionManager;

        public WaitingRoomEventHandler(
            IWaitingRoomNotifier notifier,
            IWaitingRoomSessionManager sessionManager)
        {
            _notifier = notifier;
            _sessionManager = sessionManager;
        }

        public async Task HandleCountdownTickAsync(Guid roomId, int secondsLeft)
        {
            await _notifier.NotifyCountdownTickAsync(roomId, secondsLeft);
        }

        public async Task HandleRoomExpiredAsync(Guid roomId)
        {
            _sessionManager.RemoveSession(roomId);
            await _notifier.NotifyRoomExpiredAsync(roomId);
        }

        public async Task NotifyPlayerJoinedAsync(Guid roomId, PlayerDto player, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(roomId);
            if (session == null)
                return;

            var roomDto = Domain.Mappings.WaitingRoomSessionMapper.ToDto(session);
            await _notifier.NotifyRoomUpdatedAsync(roomDto);
        }

        public async Task NotifyRoomUpdatedAsync(Guid roomId, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(roomId);
            if (session == null)
                return;

            var dto = Domain.Mappings.WaitingRoomSessionMapper.ToDto(session);
            await _notifier.NotifyRoomUpdatedAsync(dto);
        }

    }
}