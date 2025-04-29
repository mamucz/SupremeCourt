using MediatR;
using Microsoft.Extensions.Logging;
using SupremeCourt.Domain.DTOs;
using SupremeCourt.Domain.Interfaces;
using SupremeCourt.Domain.Logic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SupremeCourt.Application.CQRS.WaitingRooms.Commands
{
    public class JoinWaitingRoomHandler : IRequestHandler<JoinWaitingRoomCommand, bool>
    {
        private readonly IWaitingRoomSessionManager _sessionManager;
        private readonly IPlayerRepository _playerRepository;
        private readonly IGameService _gameService;
        private readonly IWaitingRoomNotifier _notifier;
        private readonly ILogger<JoinWaitingRoomHandler> _logger;

        public JoinWaitingRoomHandler(
            IWaitingRoomSessionManager sessionManager,
            IPlayerRepository playerRepository,
            IGameService gameService,
            IWaitingRoomNotifier notifier,
            ILogger<JoinWaitingRoomHandler> logger)
        {
            _sessionManager = sessionManager;
            _playerRepository = playerRepository;
            _gameService = gameService;
            _notifier = notifier;
            _logger = logger;
        }

        public async Task<bool> Handle(JoinWaitingRoomCommand request, CancellationToken cancellationToken)
        {
            var session = _sessionManager.GetSession(request.WaitingRoomId);
            if (session == null)
            {
                _logger.LogWarning("❌ Místnost {RoomId} neexistuje.", request.WaitingRoomId);
                return false;
            }

            if (session.Players.Any(p => p.Id == request.PlayerId))
            {
                _logger.LogInformation("ℹ️ Hráč {PlayerId} je již připojen do místnosti {RoomId}.", request.PlayerId, request.WaitingRoomId);
                return true;
            }

            if (session.IsFull)
            {
                _logger.LogWarning("⚠️ Místnost {RoomId} je plná. Hráč {PlayerId} nemůže vstoupit.", request.WaitingRoomId, request.PlayerId);
                return false;
            }

            var player = await _playerRepository.GetByIdAsync(request.PlayerId);
            if (player == null)
            {
                _logger.LogWarning("❌ Hráč {PlayerId} nebyl nalezen v databázi.", request.PlayerId);
                return false;
            }

            // Přidání hráče do session
            var success = session.TryAddPlayer(player as IPlayer);
            if (!success)
            {
                _logger.LogWarning("❌ Nepodařilo se přidat hráče {PlayerId} do místnosti {RoomId}.", request.PlayerId, request.WaitingRoomId);
                return false;
            }

            _logger.LogInformation("✅ Hráč {PlayerId} byl přidán do místnosti {RoomId}.", request.PlayerId, request.WaitingRoomId);

            // Informování ostatních klientů
            await _notifier.NotifyPlayerJoinedAsync(session.WaitingRoomId, Domain.Mappings.PlayerMapper.Instance.ToDto(player));

            return true;
        }

    }
}
