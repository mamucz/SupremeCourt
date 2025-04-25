using SupremeCourt.Domain.Interfaces;

using SupremeCourt.Application.Services;
using SupremeCourt.Domain.DTOs;

namespace SupremeCourt.Application.EventHandlers
{
    public class WaitingRoomEventHandler : IWaitingRoomEventHandler
    {
        private readonly IWaitingRoomNotifier _notifier;
        private readonly Lazy<WaitingRoomSessionManager> _sessionManager;
        private readonly IWaitingRoomRepository _repository;

        public WaitingRoomEventHandler(
            IWaitingRoomNotifier notifier,
            Lazy<WaitingRoomSessionManager> sessionManager,
            IWaitingRoomRepository repository)
        {
            _notifier = notifier;
            _sessionManager = sessionManager;
            _repository = repository;
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
            // 1.Načti aktuální stav místnosti
            var room = await _repository.GetByIdAsync(roomId,cancellationToken);
            if (room == null)
                return;

            // 2. Přemapuj na DTO
            var roomDto = Domain.Mappings.WaitingRoomMapper.Instance.ToDto(room);

            // 3. Odeslat do klientů přes SignalR
            await _notifier.NotifyRoomUpdatedAsync(roomDto);
        }
    }
}