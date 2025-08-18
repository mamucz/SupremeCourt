using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Entities;

namespace SupremeCourt.Application.CQRS.Players.Commands
{
    public class AddAiPlayerToRoomCommandHandler : IRequestHandler<AddAiPlayerToRoomCommand, bool>
    {
        private readonly IWaitingRoomSessionManager _sessionManager;
        private readonly IWaitingRoomEventHandler _eventHandler;
        private readonly ILogger<AddAiPlayerToRoomCommandHandler> _logger;

        public AddAiPlayerToRoomCommandHandler(
            IWaitingRoomSessionManager sessionManager,
            IWaitingRoomEventHandler eventHandler,
            ILogger<AddAiPlayerToRoomCommandHandler> logger)
        {
            _sessionManager = sessionManager;
            _eventHandler = eventHandler;
            _logger = logger;
        }

        public async Task<bool> Handle(AddAiPlayerToRoomCommand request, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(request.WaitingRoomId);
            if (session is null)
            {
                _logger.LogWarning("❌ AI hráč: Místnost {RoomId} neexistuje", request.WaitingRoomId);
                return false;
            }

            if (session.IsFull)
            {
                _logger.LogWarning("❌ AI hráč: Místnost {RoomId} je plná", request.WaitingRoomId);
                return false;
            }

            // Přidání přes veřejnou metodu sessionu (thread-safe uvnitř)
            var added = session.TryAddPlayer(request.player);
            if (!added)
            {
                _logger.LogInformation("ℹ️ AI hráč {Username} už v místnosti {RoomId} je", request.player.Username, request.WaitingRoomId);
                return false;
            }

            // Notifikace
            await _eventHandler.NotifyPlayerJoinedAsync(
                request.WaitingRoomId,
                Domain.Mappings.PlayerMapper.Instance.ToDto(request.player as Player),
                cancellationToken);

            _logger.LogInformation("🤖 AI hráč {Username} přidán do místnosti {RoomId}", request.player.Username, request.WaitingRoomId);
            return true;
        }
    }
}
